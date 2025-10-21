using Microsoft.Extensions.DependencyInjection;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor;

namespace Sorling.SqlConnAuthWeb.extenstions;

/// <summary>
/// Extension methods for <see cref="IMvcBuilder"/> to integrate SQL authentication routing and authorization
/// into Razor Pages applications.
/// </summary>
/// <remarks>
/// Typical usage in Program.cs:
/// <code>
/// builder.Services
/// .AddSqlConnAuthorization()
/// .AddRazorPages()
/// .AddSqlAuthRazorPageRouteConventions()
/// .AddSqlAuthFilters()
/// .AuthorizeSqlAuthRootPath();
/// </code>
/// The order shown above ensures:
/// - Razor Pages is registered before adding conventions/filters.
/// - SQL auth route parameters are appended to page routes.
/// - RedirectToPage results preserve ambient SQL auth parameters.
/// - The configured SQL auth root (if provided) is protected by the SQL auth policy.
/// </remarks>
public static class MvcBuilderExtensions
{
   /// <summary>
   /// Adds a Razor Pages route convention that rewrites page routes to include SQL authentication parameters
   /// (e.g. server, user, and optionally database name) according to the configured <see cref="SqlAuthAppPaths"/>.
   /// </summary>
   /// <param name="builder">The MVC builder to configure.</param>
   /// <returns>The same <see cref="IMvcBuilder"/> instance for chaining.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <see langword="null"/>.</exception>
   /// <remarks>
   /// This enables ambient routing so that links such as <c>&lt;a asp-page="..." /&gt;</c> and redirects
   /// can include the SQL auth parameters in the URL path according to your app's root path configuration.
   /// </remarks>
   public static IMvcBuilder AddSqlAuthRazorPageRouteConventions(this IMvcBuilder builder) {
      ArgumentNullException.ThrowIfNull(builder);

      return builder.AddRazorPagesOptions(options => {
         ServiceProvider provider = builder.Services.BuildServiceProvider();
         ISqlAuthPageRouteModelConvention convention = provider.GetRequiredService<ISqlAuthPageRouteModelConvention>();
         options.Conventions.Add(convention);
      });
   }

   /// <summary>
   /// Protects the configured SQL auth root folder (if any) with the SQL authentication policy.
   /// </summary>
   /// <param name="builder">The MVC builder to configure.</param>
   /// <returns>The same <see cref="IMvcBuilder"/> instance for chaining.</returns>
   /// <exception cref="ArgumentNullException">
   /// Thrown if <paramref name="builder"/> is <see langword="null"/> or if <see cref="SqlAuthAppPaths"/> is not registered in DI.
   /// </exception>
   /// <remarks>
   /// If <see cref="SqlAuthAppPaths.Root"/> is empty, this method is a no-op.
   /// When set, the folder is authorized with policy <see cref="SqlAuthConsts.SQLAUTHPOLICY"/>.
   /// </remarks>
   public static IMvcBuilder AuthorizeSqlAuthRootPath(this IMvcBuilder builder) {
      ArgumentNullException.ThrowIfNull(builder);
      ServiceProvider provider = builder.Services.BuildServiceProvider();
      SqlAuthAppPaths sqlauthpath = provider.GetRequiredService<SqlAuthAppPaths>();

      return !string.IsNullOrEmpty(sqlauthpath.Root)
      ? builder.AddRazorPagesOptions(options
      => options.Conventions.AuthorizeFolder(sqlauthpath.Root, SqlAuthConsts.SQLAUTHPOLICY))
      : builder;
   }

   /// <summary>
   /// Registers global MVC filters used by SQL auth integration.
   /// </summary>
   /// <param name="builder">The MVC builder to configure.</param>
   /// <returns>The same <see cref="IMvcBuilder"/> instance for chaining.</returns>
   /// <remarks>
   /// Currently adds <see cref="SqlAuthRedirectToPageFilter"/> which merges ambient SQL auth route
   /// values into <see cref="Microsoft.AspNetCore.Mvc.RedirectToPageResult"/> unless explicitly provided.
   /// Should be called after <c>AddRazorPages()</c> in the registration chain.
   /// </remarks>
   public static IMvcBuilder AddSqlAuthFilters(this IMvcBuilder builder)
      => builder.AddMvcOptions(o => o.Filters.Add(new SqlAuthRedirectToPageFilter()));
}

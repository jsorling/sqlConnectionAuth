using Microsoft.Extensions.DependencyInjection;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor;

namespace Sorling.SqlConnAuthWeb.extenstions;

/// <summary>
/// Provides extension methods for <see cref="IMvcBuilder"/> to configure SQL authentication Razor Page route conventions and authorization.
/// </summary>
public static class MvcBuilderExtensions
{
   /// <summary>
   /// Adds custom Razor Page route conventions for SQL authentication paths.
   /// </summary>
   /// <param name="builder">The MVC builder to configure.</param>
   /// <returns>The configured <see cref="IMvcBuilder"/> instance.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
   public static IMvcBuilder AddSqlAuthRazorPageRouteConventions(this IMvcBuilder builder) {
      ArgumentNullException.ThrowIfNull(builder);

      return builder.AddRazorPagesOptions(options => {
         ServiceProvider provider = builder.Services.BuildServiceProvider();
         ISqlAuthPageRouteModelConvention convention = provider.GetRequiredService<ISqlAuthPageRouteModelConvention>();
         options.Conventions.Add(convention);
      });
   }

   /// <summary>
   /// Adds authorization for the root path of SQL authentication Razor Pages.
   /// </summary>
   /// <param name="builder">The MVC builder to configure.</param>
   /// <returns>The configured <see cref="IMvcBuilder"/> instance.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null or <see cref="SqlAuthAppPaths"/> is not registered in DI.</exception>
   public static IMvcBuilder AuthorizeSqlAuthRootPath(this IMvcBuilder builder) {
      ArgumentNullException.ThrowIfNull(builder);
      ServiceProvider provider = builder.Services.BuildServiceProvider();
      SqlAuthAppPaths sqlauthpath = provider.GetRequiredService<SqlAuthAppPaths>();

      return !string.IsNullOrEmpty(sqlauthpath.Root)
          ? builder.AddRazorPagesOptions(options
              => options.Conventions.AuthorizeFolder(sqlauthpath.Root, SqlAuthConsts.SQLAUTHPOLICY))
          : builder;
   }
}

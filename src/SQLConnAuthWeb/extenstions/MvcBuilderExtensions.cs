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
   /// <param name="sqlAuthPath">The SQL authentication application path configuration.</param>
   /// <returns>The configured <see cref="IMvcBuilder"/> instance.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="sqlAuthPath"/> is null.</exception>
   public static IMvcBuilder AddSqlAuthRazorPageRouteConventions(this IMvcBuilder builder) {
      ArgumentNullException.ThrowIfNull(builder);      

      return builder.AddRazorPagesOptions(options =>
      {
         ServiceProvider provider = builder.Services.BuildServiceProvider();
         ISqlAuthPageRouteModelConvention convention = provider.GetRequiredService<ISqlAuthPageRouteModelConvention>();
         options.Conventions.Add(convention);
      });

      //return !string.IsNullOrEmpty(sqlAuthPath.Root) || !string.IsNullOrEmpty(sqlAuthPath.Tail)
      //    ? builder.AddRazorPagesOptions(options
      //        => options.Conventions.Add(new SqlAuthPageRouteModelConvention(sqlAuthPath)))
      //    : builder;
   }

   /// <summary>
   /// Adds authorization for the root path of SQL authentication Razor Pages.
   /// </summary>
   /// <param name="builder">The MVC builder to configure.</param>
   /// <param name="sqlAuthPath">The SQL authentication application path configuration.</param>
   /// <returns>The configured <see cref="IMvcBuilder"/> instance.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="sqlAuthPath"/> is null.</exception>
   public static IMvcBuilder AuthorizeSqlAuthRootPath(this IMvcBuilder builder, SqlAuthAppPaths sqlAuthPath) {
      ArgumentNullException.ThrowIfNull(builder);
      ArgumentNullException.ThrowIfNull(sqlAuthPath);

      return !string.IsNullOrEmpty(sqlAuthPath.Root)
          ? builder.AddRazorPagesOptions(options
              => options.Conventions.AuthorizeFolder(sqlAuthPath.Root, SqlAuthConsts.SQLAUTHPOLICY))
          : builder;
   }
}

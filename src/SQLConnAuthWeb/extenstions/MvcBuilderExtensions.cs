using Microsoft.Extensions.DependencyInjection;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor;

namespace Sorling.SqlConnAuthWeb.extenstions;

public static class MvcBuilderExtensions
{
   public static IMvcBuilder AddSqlAuthRazorPageRouteConventions(this IMvcBuilder builder, SqlAuthAppPaths sqlAuthPath) {
      ArgumentNullException.ThrowIfNull(builder);
      ArgumentNullException.ThrowIfNull(sqlAuthPath);

      return !string.IsNullOrEmpty(sqlAuthPath.Root) || !string.IsNullOrEmpty(sqlAuthPath.Tail)
         ? builder.AddRazorPagesOptions(options
            => options.Conventions.Add(new SqlAuthPageRouteModelConvention(sqlAuthPath)))
         : builder;
   }

   public static IMvcBuilder AuthorizeSqlAuthRootPath(this IMvcBuilder builder, SqlAuthAppPaths sqlAuthPath) {
      ArgumentNullException.ThrowIfNull(builder);
      ArgumentNullException.ThrowIfNull(sqlAuthPath);

      return !string.IsNullOrEmpty(sqlAuthPath.Root)
         ? builder.AddRazorPagesOptions(options
            => options.Conventions.AuthorizeFolder(sqlAuthPath.Root, SqlAuthConsts.SQLAUTHPOLICY))
         : builder;
   }
}

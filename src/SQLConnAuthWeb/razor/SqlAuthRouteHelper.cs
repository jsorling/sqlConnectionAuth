using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// Helpers to generate page links and redirects while automatically preserving SQL authentication
/// route parameters injected by the SQL auth route convention.
/// </summary>
public static class SqlAuthRouteHelper
{
   /// <summary>
   /// Extracts the ambient route values related to SQL authentication from the current request context.
   /// </summary>
   /// <param name="httpContext">The current HTTP context.</param>
   /// <returns>
   /// A <see cref="RouteValueDictionary"/> containing any of the following keys when present in the current route:
   /// <list type="bullet">
   /// <item><description><c>area</c> – the ambient area, if any.</description></item>
   /// <item><description><see cref="SqlAuthConsts.URLROUTEPARAMSRV"/> – the SQL Server route value.</description></item>
   /// <item><description><see cref="SqlAuthConsts.URLROUTEPARAMUSR"/> – the SQL user route value.</description></item>
   /// <item><description><see cref="SqlAuthConsts.URLROUTEPARAMDB"/> – the SQL database route value (when DB name routing is enabled).</description></item>
   /// </list>
   /// </returns>
   public static RouteValueDictionary GetSqlAuthRouteValues(HttpContext httpContext) {
      RouteValueDictionary merged = [];

      // Include ambient area if present
      if (httpContext.Request.RouteValues.TryGetValue("area", out object? area))
      {
         merged["area"] = area;
      }

      // Pull current SQL auth route values from the ambient route data
      RouteValueDictionary routevalues = httpContext.Request.RouteValues;
      if (routevalues.TryGetValue(SqlAuthConsts.URLROUTEPARAMSRV, out object? srv) && srv is string ssrv && !string.IsNullOrEmpty(ssrv))
      {
         merged[SqlAuthConsts.URLROUTEPARAMSRV] = ssrv;
      }

      if (routevalues.TryGetValue(SqlAuthConsts.URLROUTEPARAMUSR, out object? usr) && usr is string susr && !string.IsNullOrEmpty(susr))
      {
         merged[SqlAuthConsts.URLROUTEPARAMUSR] = susr;
      }

      if (routevalues.TryGetValue(SqlAuthConsts.URLROUTEPARAMDB, out object? db) && db is string sdb && !string.IsNullOrEmpty(sdb))
      {
         merged[SqlAuthConsts.URLROUTEPARAMDB] = sdb;
      }

      return merged;
   }
}

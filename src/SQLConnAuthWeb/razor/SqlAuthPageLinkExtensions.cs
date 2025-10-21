using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// Helpers to generate page links and redirects while automatically preserving SQL authentication
/// route parameters injected by the SQL auth route convention.
/// </summary>
public static class SqlAuthPageLinkExtensions
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

   /// <summary>
   /// Extracts the ambient SQL authentication route values from a <see cref="PageModel"/> instance.
   /// </summary>
   /// <param name="page">The current page model.</param>
   /// <returns>A <see cref="RouteValueDictionary"/> with the ambient area (if any) and SQL auth route parameters.</returns>
   public static RouteValueDictionary GetSqlAuthRouteValues(this PageModel page)
   => GetSqlAuthRouteValues(page.HttpContext);

   /// <summary>
   /// Merges provided route values with the ambient SQL auth route parameters from the current request.
   /// </summary>
   /// <param name="httpContext">The current HTTP context.</param>
   /// <param name="values">The base route values to merge into (can be null).</param>
   /// <returns>A new <see cref="RouteValueDictionary"/> containing the merged values.</returns>
   private static RouteValueDictionary MergeSqlAuthRouteValues(HttpContext httpContext, object? values) {
      RouteValueDictionary merged = values switch {
         null => [],
         RouteValueDictionary rvd => new RouteValueDictionary(rvd),
         _ => new RouteValueDictionary(values)
      };

      // Merge in ambient area and SQL auth params
      foreach (KeyValuePair<string, object?> kvp in GetSqlAuthRouteValues(httpContext))
      {
         merged[kvp.Key] = kvp.Value;
      }

      return merged;
   }

   /// <summary>
   /// Sets the <c>area</c> route value in the provided <see cref="RouteValueDictionary"/>.
   /// </summary>
   /// <param name="rvd">The route value dictionary to update.</param>
   /// <param name="area">The area value to set (use null to target non-area pages).</param>
   /// <returns>The same dictionary instance for chaining.</returns>
   private static RouteValueDictionary EnsureArea(RouteValueDictionary rvd, string? area) {
      rvd["area"] = area;
      return rvd;
   }

   /// <summary>
   /// Redirects to a Razor Page while preserving the ambient area (if any) and SQL auth route parameters.
   /// </summary>
   /// <param name="page">The current page.</param>
   /// <param name="pageName">The page name or path (relative or absolute).</param>
   /// <param name="routeValues">Optional additional route values to include.</param>
   /// <returns>An <see cref="IActionResult"/> that performs the redirect.</returns>
   public static IActionResult RedirectToPageWithSqlAuth(this PageModel page, string pageName, object? routeValues = null)
   => page.RedirectToPage(pageName, MergeSqlAuthRouteValues(page.HttpContext, routeValues));

   /// <summary>
   /// Redirects to a non-area (app) Razor Page while preserving SQL auth route parameters (forces <c>area = null</c>).
   /// </summary>
   /// <param name="page">The current page.</param>
   /// <param name="pageName">The page name or path (relative or absolute).</param>
   /// <param name="routeValues">Optional additional route values to include.</param>
   /// <returns>An <see cref="IActionResult"/> that performs the redirect.</returns>
   public static IActionResult RedirectToAppPageWithSqlAuth(this PageModel page, string pageName, object? routeValues = null)
   => page.RedirectToPage(pageName, EnsureArea(MergeSqlAuthRouteValues(page.HttpContext, routeValues), null));

   /// <summary>
   /// Redirects to a Razor Pages area while preserving SQL auth route parameters (forces a specific <c>area</c> value).
   /// </summary>
   /// <param name="page">The current page.</param>
   /// <param name="area">The target area name.</param>
   /// <param name="pageName">The page name or path (relative or absolute).</param>
   /// <param name="routeValues">Optional additional route values to include.</param>
   /// <returns>An <see cref="IActionResult"/> that performs the redirect.</returns>
   public static IActionResult RedirectToAreaPageWithSqlAuth(this PageModel page, string area, string pageName, object? routeValues = null)
   => page.RedirectToPage(pageName, EnsureArea(MergeSqlAuthRouteValues(page.HttpContext, routeValues), area));

   /// <summary>
   /// Generates a URL to a Razor Page while preserving the ambient area (if any) and SQL auth route parameters.
   /// </summary>
   /// <param name="url">The URL helper.</param>
   /// <param name="pageName">The page name or path (relative or absolute).</param>
   /// <param name="routeValues">Optional additional route values to include.</param>
   /// <returns>The generated URL or null if no route matches.</returns>
   public static string? PageWithSqlAuth(this IUrlHelper url, string pageName, object? routeValues = null)
   => url.Page(pageName, MergeSqlAuthRouteValues(url.ActionContext.HttpContext, routeValues));

   /// <summary>
   /// Generates a URL to a non-area (app) Razor Page while preserving SQL auth route parameters (forces <c>area = null</c>).
   /// </summary>
   /// <param name="url">The URL helper.</param>
   /// <param name="pageName">The page name or path (relative or absolute).</param>
   /// <param name="routeValues">Optional additional route values to include.</param>
   /// <returns>The generated URL or null if no route matches.</returns>
   public static string? PageAppWithSqlAuth(this IUrlHelper url, string pageName, object? routeValues = null)
   => url.Page(pageName, EnsureArea(MergeSqlAuthRouteValues(url.ActionContext.HttpContext, routeValues), null));

   /// <summary>
   /// Generates a URL to a Razor Pages area while preserving SQL auth route parameters (forces a specific <c>area</c> value).
   /// </summary>
   /// <param name="url">The URL helper.</param>
   /// <param name="area">The target area name.</param>
   /// <param name="pageName">The page name or path (relative or absolute).</param>
   /// <param name="routeValues">Optional additional route values to include.</param>
   /// <returns>The generated URL or null if no route matches.</returns>
   public static string? PageInAreaWithSqlAuth(this IUrlHelper url, string area, string pageName, object? routeValues = null)
   => url.Page(pageName, EnsureArea(MergeSqlAuthRouteValues(url.ActionContext.HttpContext, routeValues), area));
}

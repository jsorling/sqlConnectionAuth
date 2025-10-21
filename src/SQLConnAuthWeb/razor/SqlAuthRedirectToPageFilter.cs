using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// Result filter that preserves ambient SQL authentication route parameters for Razor Pages redirects.
/// </summary>
/// <remarks>
/// When a <see cref="RedirectToPageResult"/> is executed, this filter merges the current request's
/// ambient route values for SQL auth (<see cref="SqlAuthConsts.URLROUTEPARAMSRV"/>,
/// <see cref="SqlAuthConsts.URLROUTEPARAMUSR"/>, <see cref="SqlAuthConsts.URLROUTEPARAMDB"/>) and
/// ambient <c>area</c> (if present) into the redirect's <see cref="RedirectToPageResult.RouteValues"/>.
/// Explicit values provided by the action override ambient values.
/// </remarks>
public sealed class SqlAuthRedirectToPageFilter : IResultFilter
{
   /// <summary>
   /// No-op. Included to satisfy <see cref="IResultFilter"/>.
   /// </summary>
   /// <param name="context">The executed result context.</param>
   public void OnResultExecuted(ResultExecutedContext context) { }

   /// <summary>
   /// Merges ambient SQL auth route values (and area) into <see cref="RedirectToPageResult"/>
   /// before it executes, unless already explicitly provided.
   /// </summary>
   /// <param name="context">The result executing context.</param>
   public void OnResultExecuting(ResultExecutingContext context) {
      if (context.Result is RedirectToPageResult r && context.HttpContext is not null)
      {
         // Start with ambient area and SQL auth params
         RouteValueDictionary merged = SqlAuthRouteHelper.GetSqlAuthRouteValues(context.HttpContext);

         // Merge explicit values from the redirect result (explicit wins)
         if (r.RouteValues is not null)
         {
            foreach (KeyValuePair<string, object?> kv in r.RouteValues)
            {
               merged[kv.Key] = kv.Value;
            }
         }

         r.RouteValues = merged;
      }
   }
}

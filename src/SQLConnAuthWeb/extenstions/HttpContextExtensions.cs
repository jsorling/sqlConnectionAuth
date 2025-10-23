using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Sorling.SqlConnAuthWeb.extenstions;

/// <summary>
/// Extension methods for <see cref="HttpContext"/> to support SQL authentication scenarios.
/// </summary>
public static class HttpContextExtensions
{
   /// <summary>
   /// Signs out the current user and clears any authentication state for SQL authentication.
   /// </summary>
   /// <param name="httpContext">The current HTTP context.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public static async Task SqlAuthSignoutAsync(this HttpContext httpContext)
       => await httpContext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

   /// <summary>
   /// Returns a new query string with the specified key and value added or updated, based on the current request's query string.
   /// </summary>
   /// <param name="httpContext">The current HTTP context.</param>
   /// <param name="key">The query parameter key to add or update.</param>
   /// <param name="value">The value to set for the query parameter.</param>
   /// <returns>A URI-escaped query string (including the leading '?') with the updated parameter.</returns>
   public static string WithQueryParameter(this HttpContext httpContext, string key, string? value) {
      Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(httpContext.Request.QueryString.Value ?? "");
      Dictionary<string, string?> dict = new(StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, StringValues> kvp in query)
         dict[kvp.Key] = kvp.Value;

      dict[key] = value;
      string newquery = QueryHelpers.AddQueryString("", dict);
      return newquery;
   }
}

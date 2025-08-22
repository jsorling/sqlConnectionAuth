using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.passwords;

namespace Sorling.SqlConnAuthWeb.extenstions;

public static class HttpContextExtensions
{
   /// <summary>
   /// Signs out the current user and clears any authentication state.
   /// </summary>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public static async Task SqlAuthSignoutAsync(this HttpContext httpContext)
       => await httpContext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

   /// <summary>
   /// Gets the SQL Server name associated with the current authentication context.
   /// </summary>
   public static string GetSqlAuthServer(this HttpContext httpContext) => httpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMSRV]
      as string ?? throw new ApplicationException("SQL server cannot be null or empty on route.");

   /// <summary>
   /// Gets the user name associated with the current authentication context.
   /// </summary>
   public static string GetSqlAuthUserName(this HttpContext httpContext)
      => httpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMUSR] as string
         ?? throw new ApplicationException("SQL username cannot be null or empty on route.");

   /// <summary>
   /// Gets the stored secrets used for SQL authentication, such as credentials or sensitive connection information.
   /// Returns <c>null</c> if no secrets are available in the current authentication context.
   /// </summary>
   public static SqlAuthStoredSecrets? GetSqlAuthStoredSecrets(this HttpContext httpContext)
      => httpContext.Items[typeof(SqlAuthStoredSecrets)] as SqlAuthStoredSecrets;

   /// <summary>
   /// Gets the SQL connection string for the specified database, or the default if no database is specified.
   /// </summary>
   /// <param name="database">The name of the database, or null for the default.</param>
   /// <returns>The connection string, or null if not available.</returns>
   public static string? GetSqlAuthGetConnectionString(this HttpContext httpContext, string? database = null)
      => httpContext.GetSqlAuthConnectionstringProvider().ConnectionString(database);

   /// <summary>
   /// Gets a <see cref="SqlAuthConnectionstringProvider"/> for the current authentication context.
   /// </summary>
   /// <param name="httpContext">The current HTTP context.</param>
   /// <returns>
   /// An instance of <see cref="SqlAuthConnectionstringProvider"/> initialized with the SQL server, user name, and stored secrets from the current context.
   /// </returns>
   /// <exception cref="ApplicationException">
   /// Thrown if the stored secrets are not available in the current authentication context.
   /// </exception>
   public static SqlAuthConnectionstringProvider GetSqlAuthConnectionstringProvider(this HttpContext httpContext)
      => httpContext.GetSqlAuthStoredSecrets() is SqlAuthStoredSecrets storedsecrets
         ? new SqlAuthConnectionstringProvider(httpContext.GetSqlAuthServer(), httpContext.GetSqlAuthUserName(), storedsecrets)
         : throw new ApplicationException("SQL connection string provider cannot be created without stored secrets.");

   /// <summary>
   /// Gets the value of the temporary password key from the query parameters.
   /// </summary>
   /// <param name="httpContext">The current HTTP context.</param>
   /// <returns>The value of the temporary password key if present; otherwise, null.</returns>
   public static string? GetSqlAuthTempPwdKeyFromQuery(this HttpContext httpContext)
      => httpContext.Request.Query[SqlAuthConsts.QUERYPARAMTMPPWDKEY].FirstOrDefault();

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

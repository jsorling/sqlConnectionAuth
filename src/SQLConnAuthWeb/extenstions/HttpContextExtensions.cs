using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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

   public static SqlAuthConnectionstringProvider GetSqlAuthConnectionstringProvider(this HttpContext httpContext) 
      => httpContext.GetSqlAuthStoredSecrets() is SqlAuthStoredSecrets storedsecrets
         ? new SqlAuthConnectionstringProvider(httpContext.GetSqlAuthServer(), httpContext.GetSqlAuthUserName(), storedsecrets)
         : throw new ApplicationException("SQL connection string provider cannot be created without stored secrets.");
}

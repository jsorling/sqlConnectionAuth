using Sorling.SqlConnAuthWeb.authentication.passwords;
using Microsoft.AspNetCore.Mvc;
using Sorling.SqlConnAuthWeb.razor;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Defines the contract for SQL authentication context, providing access to connection details, stored secrets, and connection string generation.
/// </summary>
public interface ISqlAuthContext
{
   /// <summary>
   /// Gets the SQL Server name or address.
   /// </summary>
   string SqlServer { get; }

   /// <summary>
   /// Gets the SQL user name for authentication.
   /// </summary>
   string SqlUserName { get; }

   /// <summary>
   /// Gets the SQL database name.
   /// </summary>
   string? SqlDBName { get; }

   /// <summary>
   /// Gets the stored secrets containing password and connection options.
   /// </summary>
   SqlAuthStoredSecrets? StoredSecrets { get; }

   /// <summary>
   /// Gets the provider for building SQL connection strings.
   /// </summary>
   SqlAuthConnectionstringProvider ConnectionstringProvider { get; }

   /// <summary>
   /// Gets a SQL connection string for the specified database or the default database if none is specified.
   /// </summary>
   /// <param name="database">The database name, or null for the default database.</param>
   /// <returns>A SQL connection string for the specified or default database.</returns>
   string? GetConnectionString(string? database = null);

   /// <summary>
   /// Gets the temporary password key from the query string, if present.
   /// </summary>
   string? TempPwdKeyFromQuery { get; }

   /// <summary>
   /// Gets the temporary password key from the route, if present.
   /// </summary>
   string? TempPwdKey { get; }

   /// <summary>
   /// Gets an <see cref="ActionResult"/> that redirects to the SelectDB page with a generated temporary password key in the route.
   /// This allows unauthenticated users to select a database in a secure, temporary context.
   /// </summary>
   RedirectToSelectDBWithTempPwdKeyResult GetRedirectToSelectDBActionResult();

   /// <summary>
   /// Gets the application path configuration for SQL authentication, including root and tail segments.
   /// </summary>
   SqlAuthAppPaths AppPaths { get; }
}

using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Defines methods and properties for SQL authentication services, including authentication, connection string management, and database listing.
/// </summary>
public interface ISqlAuthService
{
   /// <summary>
   /// Authenticates a user using the provided SQL authentication request.
   /// </summary>
   /// <param name="request">The authentication request containing credentials and connection details.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
   public Task<SqlAuthenticationResult> AuthenticateAsync(SQLAuthenticateRequest request);

   /// <summary>
   /// Signs out the current user and clears any authentication state.
   /// </summary>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public Task SignoutAsync();

   /// <summary>
   /// Gets the SQL connection string for the specified database, or the default if no database is specified.
   /// </summary>
   /// <param name="database">The name of the database, or null for the default.</param>
   /// <returns>The connection string, or null if not available.</returns>
   public string? GetConnectionString(string? database = null);

   /// <summary>
   /// Retrieves a list of available databases for the current SQL connection.
   /// </summary>
   /// <returns>A task that represents the asynchronous operation. The task result contains a collection of database information.</returns>
   public Task<IEnumerable<SqlConnectionHelper.DBName>> GetDBsAsync();

   /// <summary>
   /// Gets the SQL authentication options used by the service.
   /// </summary>
   public SqlAuthOptions Options { get; }

   /// <summary>
   /// Gets the URI-escaped path for the current SQL connection context.
   /// </summary>
   public string UriEscapedPath { get; }

   /// <summary>
   /// Gets the SQL Server name associated with the current authentication context.
   /// </summary>
   public string SQLServer { get; }

   /// <summary>
   /// Gets the user name associated with the current authentication context.
   /// </summary>
   public string UserName { get; }
}

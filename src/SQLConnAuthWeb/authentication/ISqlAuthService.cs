using Sorling.SqlConnAuthWeb.authentication.passwords;
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
   /// Gets the SQL authentication options used by the service.
   /// </summary>
   public SqlAuthOptions Options { get; }

   /// <summary>
   /// Gets the URI-escaped path for the current SQL connection context.
   /// </summary>
   public string UriEscapedPath { get; }

   /// <summary>
   /// Tests SQL authentication using a temporary password and optional database name, without affecting the current authentication state.
   /// </summary>
   /// <param name="sqlAuthTempPasswordInfo">The temporary password information for SQL authentication.</param>
   /// <param name="dbName">The name of the database to test authentication against, or null for the default database.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
   public Task<SqlAuthenticationResult> TestAuthenticateAsync(SqlAuthTempPasswordInfo sqlAuthTempPasswordInfo, string? dbName);

   /// <summary>
   /// Tests SQL authentication using a specified key and optional database name, without affecting the current authentication state.
   /// The key is only peeked and not removed.
   /// </summary>
   /// <param name="key">The key used to retrieve temporary authentication information. The key is not removed (peek only).</param>
   /// <param name="dbName">The name of the database to test authentication against, or null for the default database.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
   public Task<SqlAuthenticationResult> TestAuthenticateAsync(string key, string? dbName);
}

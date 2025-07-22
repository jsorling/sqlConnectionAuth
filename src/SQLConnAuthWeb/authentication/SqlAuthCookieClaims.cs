namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents the claims stored in an authentication cookie for SQL connections.
/// </summary>
/// <param name="Server">The SQL server name associated with the authentication context.</param>
/// <param name="UserName">The user name associated with the authentication context.</param>
/// <param name="SecretStoreKey">The key used to retrieve stored secrets for authentication.</param>
public record SqlAuthCookieClaims(string Server, string UserName, string SecretStoreKey);


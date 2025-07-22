namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents a request to authenticate using SQL credentials and connection options.
/// </summary>
/// <param name="Password">The password to use for authentication.</param>
/// <param name="TrustServerCertificate">Indicates whether to trust the SQL Server certificate. Default is false.</param>
public record SQLAuthenticateRequest(string Password, bool TrustServerCertificate = false);

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents a request to authenticate using SQL credentials and connection options.
/// </summary>
/// <param name="Password">The password to use for authentication.</param>
/// <param name="DBName">Optional database name to authenticate against when database name routing is enabled; otherwise null.</param>
/// <param name="TrustServerCertificate">Indicates whether to trust the SQL Server certificate. Default is false.</param>
/// <param name="NoDataBaseFilter">If true, skip DB name allow-list validation during authentication.</param>
public record SQLAuthenticateRequest(string Password, string? DBName, bool TrustServerCertificate = false, bool NoDataBaseFilter = false);

namespace Sorling.SqlConnAuthWeb.authentication.validation;

/// <summary>
/// Represents a request to validate SQL authentication, including connection and credential information.
/// </summary>
/// <param name="Datasource">The SQL Server data source (server address or name).</param>
/// <param name="UserName">The user name for authentication.</param>
/// <param name="Password">The password for authentication.</param>
/// <param name="TrustServerCertificate">Indicates whether to trust the SQL Server certificate. Default is false.</param>
public record SqlAuthValidationRequest(string Datasource, string UserName, string Password, bool TrustServerCertificate = false);

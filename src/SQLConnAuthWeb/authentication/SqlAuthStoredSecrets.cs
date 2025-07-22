namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents stored secrets for SQL authentication, including password, certificate trust, and rule revalidation time.
/// </summary>
/// <param name="Password">The password used for SQL authentication.</param>
/// <param name="TrustServerCertificate">Indicates whether to trust the SQL Server certificate.</param>
/// <param name="RuleReValidationAfter">The UTC time after which rule revalidation is required, or null if not set.</param>
public record SqlAuthStoredSecrets(string Password, bool TrustServerCertificate, DateTime? RuleReValidationAfter);

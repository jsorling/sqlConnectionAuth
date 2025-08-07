namespace Sorling.SqlConnAuthWeb.authentication.passwords;

/// <summary>
/// Represents stored secrets for SQL authentication, including password, certificate trust, and rule revalidation time.
/// </summary>
/// <param name="Password">The password used for SQL authentication.</param>
/// <param name="TrustServerCertificate">Indicates whether to trust the SQL Server certificate.</param>
/// <param name="RuleReValidationAfter">The UTC time after which rule revalidation is required, or null if not set.</param>
/// <param name="DBName">Optional database name for the SQL connection, or null if not specified.</param>
public record SqlAuthStoredSecrets(string Password, bool TrustServerCertificate, DateTime? RuleReValidationAfter, string? DBName = null);

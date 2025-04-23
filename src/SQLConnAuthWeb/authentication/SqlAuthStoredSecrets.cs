namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlAuthStoredSecrets(string Password, bool TrustServerCertificate, DateTime? RuleReValidationAfter);

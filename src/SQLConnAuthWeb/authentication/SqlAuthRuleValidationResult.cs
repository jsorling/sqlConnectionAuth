namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlAuthRuleValidationResult(Exception? Exception, SqlAuthStoredSecrets? StoredSecrets);

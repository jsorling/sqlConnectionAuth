namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents the result of validating SQL authentication rules, including any exception and the validated secrets.
/// </summary>
/// <param name="Exception">The exception encountered during rule validation, if any; otherwise, null.</param>
/// <param name="StoredSecrets">The validated and possibly updated stored secrets, or null if validation failed.</param>
public record SqlAuthRuleValidationResult(Exception? Exception, SqlAuthStoredSecrets? StoredSecrets);

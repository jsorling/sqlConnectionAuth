namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Defines a method for validating SQL authentication rules based on a validation request.
/// </summary>
public interface ISqlAuthRuleValidator
{
    /// <summary>
    /// Validates the provided SQL authentication validation request against defined rules.
    /// </summary>
    /// <param name="request">The validation request containing the necessary information for rule validation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validation result.</returns>
    public Task<SqlAuthRuleValidationResult> ValidateAsync(SqlAuthValidationRequest request);
}

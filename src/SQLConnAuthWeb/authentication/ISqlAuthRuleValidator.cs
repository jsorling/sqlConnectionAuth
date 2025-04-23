namespace Sorling.SqlConnAuthWeb.authentication;

public interface ISqlAuthRuleValidator
{
   public Task<SqlAuthRuleValidationResult> ValidateAsync(SqlAuthValidationRequest request);
}

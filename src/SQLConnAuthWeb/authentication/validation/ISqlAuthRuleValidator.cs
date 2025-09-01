namespace Sorling.SqlConnAuthWeb.authentication.validation;

/// <summary>
/// Defines a contract for validating SQL authentication rules based on validation requests and database names.
/// </summary>
public interface ISqlAuthRuleValidator
{
   /// <summary>
   /// Validates the provided SQL authentication validation request against defined rules.
   /// </summary>
   /// <param name="request">The validation request containing the necessary information for rule validation.</param>
   /// <param name="dbName">The name of the database to validate, or <c>null</c> if not applicable.</param>
   /// <returns>
   /// A task that represents the asynchronous operation. The task result contains the validation result as a <see cref="SqlAuthRuleValidationResult"/>.
   /// </returns>
   Task<SqlAuthRuleValidationResult> ValidateConnectionAsync(SqlAuthValidationRequest request, string? dbName);

   /// <summary>
   /// Validates the specified database name against the defined authentication rules.
   /// </summary>
   /// <param name="databaseName">The name of the database to validate.</param>
   /// <returns>
   /// A task that represents the asynchronous operation. The task result is <c>true</c> if the database is valid; otherwise, <c>false</c>.
   /// </returns>
   Task<bool> ValidateDatabaseAsync(string databaseName);
}

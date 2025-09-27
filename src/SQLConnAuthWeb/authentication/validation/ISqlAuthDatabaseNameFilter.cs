namespace Sorling.SqlConnAuthWeb.authentication.validation;

/// <summary>
/// Provides an abstraction for validating and filtering SQL database names according to configured allow/deny rules.
/// Implementations are used by <see cref="SqlAuthRuleValidator"/> and <see cref="dbaccess.SqlAuthDBAccess"/> to enforce UI and security constraints.
/// </summary>
public interface ISqlAuthDatabaseNameFilter
{
   /// <summary>
   /// Determines if a database name is allowed based on allow/deny rules.
   /// </summary>
   /// <param name="databaseName">The database name to validate.</param>
   /// <returns>True if allowed, false otherwise.</returns>
   bool IsAllowed(string databaseName);

   /// <summary>
   /// Returns the list of allowed database names from a given set, based on allow/deny rules.
   /// </summary>
   /// <param name="databaseNames">The database names to filter.</param>
   /// <returns>Allowed database names.</returns>
   IEnumerable<string> ListAllowed(IEnumerable<string> databaseNames);
}

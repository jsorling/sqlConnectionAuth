using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.authentication.validation;

/// <summary>
/// Validates database names using allow/deny rules from SqlAuthOptions, supporting wildcards.
/// </summary>
public class SqlAuthDatabaseNameFilter(IOptionsMonitor<SqlAuthOptions> optionsMonitor) : ISqlAuthDatabaseNameFilter
{
   private readonly IOptionsMonitor<SqlAuthOptions> _optionsMonitor = optionsMonitor;

   private SqlAuthOptions Options => _optionsMonitor.CurrentValue;

   /// <summary>
   /// Determines if a database name is allowed based on allow/deny rules and wildcards.
   /// </summary>
   /// <param name="databaseName">The database name to validate.</param>
   /// <returns>True if allowed, false otherwise.</returns>
   public bool IsAllowed(string databaseName) 
      => !string.IsNullOrWhiteSpace(databaseName) && ListAllowed([databaseName]).Any();

   /// <summary>
   /// Returns the list of allowed database names from a given set, based on allow/deny rules and wildcards.
   /// </summary>
   /// <param name="databaseNames">The database names to filter.</param>
   /// <returns>Allowed database names.</returns>
   public IEnumerable<string> ListAllowed(IEnumerable<string> databaseNames) {
      if (databaseNames == null)
         yield break;
      CaseInsensitiveStringSet denypatterns = Options.ExcludeDatabaseFilter;
      CaseInsensitiveStringSet allowpatterns = Options.IncludeDatabaseFilter;
      // Remove denied databases (deny always takes precedence)
      IEnumerable<string> notdenied = databaseNames;
      foreach (string denypattern in denypatterns)
         notdenied = notdenied.FilterWithSearchPattern(denypattern, caseSensitive: false, negative: true);

      // If allow list is empty, all not-denied are allowed
      if (allowpatterns.Count == 0)
      {
         foreach (string db in notdenied)
            yield return db;
         yield break;
      }

      // Otherwise, only those matching any allow pattern
      HashSet<string> allowed = new(StringComparer.OrdinalIgnoreCase);
      foreach (string allowpattern in allowpatterns)
      {
         foreach (string db in notdenied.FilterWithSearchPattern(allowpattern, caseSensitive: false))
            _ = allowed.Add(db);
      }

      foreach (string db in allowed)
         yield return db;
   }
}

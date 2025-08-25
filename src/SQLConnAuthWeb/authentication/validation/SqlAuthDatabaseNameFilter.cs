using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.extenstions;

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
   public IEnumerable<string> ListAllowed(IEnumerable<string> databaseNames) 
      => databaseNames.FilterWithSearchPatterns(Options.ExcludeDatabaseFilter, negative: true)
         .FilterWithSearchPatterns(Options.IncludeDatabaseFilter);
}

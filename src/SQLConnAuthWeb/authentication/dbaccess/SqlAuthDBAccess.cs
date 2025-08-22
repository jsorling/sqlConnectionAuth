using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.authentication.dbaccess;

/// <summary>
/// Provides an implementation of <see cref="ISqlAuthDBAccess"/> for accessing SQL Server databases.
/// </summary>
public class SqlAuthDBAccess(ISqlAuthDatabaseNameFilter databaseNameFilter) : ISqlAuthDBAccess
{
   private readonly ISqlAuthDatabaseNameFilter _databaseNameValidator
      = databaseNameFilter ?? throw new ArgumentNullException(nameof(databaseNameFilter));

   /// <inheritdoc/>
   public async Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthConnectionstringProvider sca) {
      IEnumerable<ISqlDatabase> alldatabases = await SqlConnectionHelper.GetDatabasesAsync(sca);
      // Filter using _databaseNameValidator
      IEnumerable<string> allowednames = _databaseNameValidator.ListAllowed(alldatabases.Select(db => db.Name));
      return alldatabases.Where(db => allowednames.Contains(db.Name));
   }
}

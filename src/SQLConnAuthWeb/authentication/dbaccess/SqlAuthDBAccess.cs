using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.authentication.dbaccess;

/// <summary>
/// Provides an implementation of <see cref="ISqlAuthDBAccess"/> for accessing SQL Server databases.
/// </summary>
public class SqlAuthDBAccess : ISqlAuthDBAccess
{
   /// <inheritdoc/>
   public async Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthConnectionstringProvider sca)
      => await SqlConnectionHelper.GetDatabasesAsync(sca);
}

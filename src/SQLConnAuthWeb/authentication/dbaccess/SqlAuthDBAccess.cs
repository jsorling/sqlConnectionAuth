using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.authentication.dbaccess;

public class SqlAuthDBAccess : ISqlAuthDBAccess
{
   public async Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthConnectionstringProvider sca)
      => await SqlConnectionHelper.GetDatabasesAsync(sca);
}

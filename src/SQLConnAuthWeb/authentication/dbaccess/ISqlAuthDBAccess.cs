namespace Sorling.SqlConnAuthWeb.authentication.dbaccess;

public interface ISqlAuthDBAccess
{
   Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthConnectionstringProvider sca);
}

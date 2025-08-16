namespace Sorling.SqlConnAuthWeb.authentication.dbaccess;

/// <summary>
/// Defines a contract for accessing SQL Server databases using a given authentication context.
/// </summary>
public interface ISqlAuthDBAccess
{
   /// <summary>
   /// Retrieves a list of databases from the SQL Server using the provided connection string provider.
   /// </summary>
   /// <param name="sca">The SQL authentication connection string provider.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains a collection of database result objects.</returns>
   Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthConnectionstringProvider sca);
}

using Microsoft.Data.SqlClient;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;

namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Provides helper methods for SQL Server connections, including database listing, version retrieval, and connection testing.
/// </summary>
public class SqlConnectionHelper
{
   /// <summary>
   /// Retrieves a list of databases from the SQL Server using the provided connection string provider.
   /// </summary>
   /// <param name="sca">The SQL authentication connection string provider.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains a collection of database result objects.</returns>
   public static async Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthConnectionstringProvider sca) {
      List<ISqlDatabase> results = [];
      string connstr = sca.ConnectionString("master");
      using (SqlConnection conn = new(connstr))
      using (SqlCommand cmd = new(
         "select name from sys.databases where has_dbaccess(name) = 1 order by case when owner_sid = 0x01 then 1 else 2 end, name"
         , conn))
      {
         await conn.OpenAsync();
         using SqlDataReader reader = await cmd.ExecuteReaderAsync();
         while (await reader.ReadAsync())
         {
            results.Add(new SqlDatabase(reader.GetString(0)));
         }
      }

      return results;
   }

   /// <summary>
   /// Retrieves the SQL Server version string using the provided connection string provider.
   /// </summary>
   /// <param name="sca">The SQL authentication connection string provider.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the SQL Server version string, or null if not available.</returns>
   public static async Task<string?> GetSqlServerVersionAsync(SqlAuthConnectionstringProvider sca) {
      string connstr = sca.ConnectionString();
      using SqlConnection conn = new(connstr);
      using SqlCommand cmd = new("select @@version", conn);
      await conn.OpenAsync();
      object? result = await cmd.ExecuteScalarAsync();
      return result?.ToString();
   }

   /// <summary>
   /// Attempts to connect to the SQL Server and returns the result, including success status, exception, and version.
   /// </summary>
   /// <param name="sca">The SQL authentication connection string provider.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
   public static async Task<SqlAuthenticationResult> TryConnectWithResultAsync(SqlAuthConnectionstringProvider sca) {
      string? version = null;
      bool success = true;
      Exception? exception = null;

      try
      {
         version = await GetSqlServerVersionAsync(sca);
      }
      catch (Exception ex)
      {
         exception = ex;
         success = false;
      }

      return new(success, exception, version);
   }
}

using Sorling.SqlExec.mapper.commands;
using Sorling.SqlExec.mapper.results;
using Sorling.SqlExec.runner;
using System.Data;

namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlConnectionHelper
{
   private record VersionCommand() : SqlExecBaseCommand
   {
      public override CommandType SqlExecCommandType => CommandType.Text;

      public override string SqlExecSqlText => "select @@version";
   }

   public record ListDBCmd() : SqlExecBaseCommand
   {

      public record ListDBRes(string Name) : SqlExecBaseResult;

      public override CommandType SqlExecCommandType => CommandType.Text;

      public override string SqlExecSqlText => "select name from sys.databases order by case when owner_sid = 0x01 then 1 else 2 end, name";
   }

   public static async Task<IEnumerable<ListDBCmd.ListDBRes>> GetDbsAsync(SqlConnAuthenticationData sca) {
      SqlExecRunner runner = new(sca.ConnectionString("master"));
      return await runner.QueryAsync<ListDBCmd.ListDBRes, ListDBCmd>(new());
   }

   public static async Task<string?> GetVerionAsync(SqlConnAuthenticationData sca) {
      SqlExecRunner runner = new(sca.ConnectionString());
      return await runner.ExecuteScalarAsync<string, VersionCommand>(new());
   }

   public static async Task<SqlConnAuthenticationResult> TryConnectWithResultAsync(SqlConnAuthenticationData sca) {
      string? version = null;
      bool success = true;
      Exception? exception = null;

      try {
         version = await GetVerionAsync(sca);
      }
      catch (Exception ex) {
         exception = ex;
         success = false;
      }

      return new(success, exception, version);
   }
}

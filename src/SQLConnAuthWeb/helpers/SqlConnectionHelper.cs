using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlExec.mapper.commands;
using Sorling.SqlExec.mapper.results;
using Sorling.SqlExec.runner;
using System.Data;

namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Provides helper methods for SQL Server connections, including database listing, version retrieval, and connection testing.
/// </summary>
public class SqlConnectionHelper
{
    /// <summary>
    /// Represents a command to retrieve the SQL Server version.
    /// </summary>
    private record VersionCommand() : SqlExecBaseCommand
    {
        /// <inheritdoc/>
        public override CommandType SqlExecCommandType => CommandType.Text;

        /// <inheritdoc/>
        public override string SqlExecSqlText => "select @@version";
    }

    /// <summary>
    /// Represents a command to list all databases on the SQL Server.
    /// </summary>
    public record ListDBCmd() : SqlExecBaseCommand
    {
        /// <summary>
        /// Represents a result row for a listed database.
        /// </summary>
        /// <param name="Name">The name of the database.</param>
        public record ListDBRes(string Name) : SqlExecBaseResult;

        /// <inheritdoc/>
        public override CommandType SqlExecCommandType => CommandType.Text;

        /// <inheritdoc/>
        public override string SqlExecSqlText => "select name from sys.databases order by case when owner_sid = 0x01 then 1 else 2 end, name";
    }

    /// <summary>
    /// Retrieves a list of databases from the SQL Server using the provided connection string provider.
    /// </summary>
    /// <param name="sca">The SQL authentication connection string provider.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of database result objects.</returns>
    public static async Task<IEnumerable<ListDBCmd.ListDBRes>> GetDbsAsync(SqlAuthConnectionstringProvider sca) {
        SqlExecRunner runner = new(sca.ConnectionString("master"));
        return await runner.QueryAsync<ListDBCmd.ListDBRes, ListDBCmd>(new());
    }

    /// <summary>
    /// Retrieves the SQL Server version string using the provided connection string provider.
    /// </summary>
    /// <param name="sca">The SQL authentication connection string provider.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the SQL Server version string, or null if not available.</returns>
    public static async Task<string?> GetVerionAsync(SqlAuthConnectionstringProvider sca) {
        SqlExecRunner runner = new(sca.ConnectionString());
        return await runner.ExecuteScalarAsync<string, VersionCommand>(new());
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
            version = await GetVerionAsync(sca);
        }
        catch (Exception ex)
        {
            exception = ex;
            success = false;
        }

        return new(success, exception, version);
    }
}

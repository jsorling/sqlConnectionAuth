namespace Sorling.SqlConnAuthWeb.authentication;

public interface ISqlConnAuthenticationService
{
   public Task<SqlConnAuthenticationResult> AuthenticateAsync(string sqlServer, string userName, string password);

   public Task SignoutAsync();

   public string Version { get; }

   public SqlConnAuthenticationData SQLConnAuthenticationData { get; }

   public string SQLConnectionString(string? db = null);

   public Task<IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes>> GetDBsAsync();

   public object RouteValues { get; }

   public string UriEscapedPath { get; }

   public SqlConnAuthenticationOptions Options { get; }
}

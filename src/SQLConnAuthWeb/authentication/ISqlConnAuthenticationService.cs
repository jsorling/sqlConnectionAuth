namespace Sorling.SqlConnAuthWeb.authentication;

public interface ISqlConnAuthenticationService
{
   public Task<SqlConnAuthenticationResult> AuthenticateAsync(string sqlServer, string userName, string password);

   public Task SignoutAsync();

   public string Version { get; }

   public SqlConnAuthenticationData SqlConnAuthenticationData { get; }

   public string SqlConnectionString(string? db = null);

   public Task<IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes>> GetDBsAsync();

   public object RouteValues { get; }

   public string UriEscapedPath(string? server = null, string? user = null);

   public SqlConnAuthenticationOptions Options { get; }
}

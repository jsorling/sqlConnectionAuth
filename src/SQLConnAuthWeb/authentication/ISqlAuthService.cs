using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.authentication;

public interface ISqlAuthService
{
   public Task<SqlAuthenticationResult> AuthenticateAsync(SQLAuthenticateRequest request);

   public Task SignoutAsync();

   public string? GetConnectionString(string? database = null);

   public Task<IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes>> GetDBsAsync();

   public SqlAuthOptions Options { get; }

   public string UriEscapedPath { get; }

   public string SQLServer { get; }

   public string UserName { get; }
}

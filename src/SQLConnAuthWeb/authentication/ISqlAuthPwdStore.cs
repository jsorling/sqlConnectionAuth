namespace Sorling.SqlConnAuthWeb.authentication;

public interface ISqlAuthPwdStore
{
   public Task<string> StoreAsync(SqlAuthStoredSecrets storedSecrets);

   public Task RenewAsync(string key, SqlAuthStoredSecrets storedSecrets);

   public Task<SqlAuthStoredSecrets?> RetrieveAsync(string key);

   public Task RemoveAsync(string key);
}

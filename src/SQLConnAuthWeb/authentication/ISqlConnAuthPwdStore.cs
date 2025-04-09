namespace Sorling.SqlConnAuthWeb.authentication;

public interface ISqlConnAuthPwdStore
{
   public Task<string> StoreAsync(SqlConnAuthStoredSecrets storedSecrets);

   public Task RenewAsync(string key, SqlConnAuthStoredSecrets storedSecrets);

   public Task<SqlConnAuthStoredSecrets?> RetrieveAsync(string key);

   public Task RemoveAsync(string key);
}

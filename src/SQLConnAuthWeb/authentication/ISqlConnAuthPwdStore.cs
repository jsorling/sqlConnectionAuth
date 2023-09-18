namespace Sorling.SqlConnAuthWeb.authentication;

public interface ISqlConnAuthPwdStore
{
   public Task<string> StoreAsync(string pwd);

   public Task RenewAsync(string key, string pwd);

   public Task<string?> RetrieveAsync(string key);

   public Task RemoveAsync(string key);
}

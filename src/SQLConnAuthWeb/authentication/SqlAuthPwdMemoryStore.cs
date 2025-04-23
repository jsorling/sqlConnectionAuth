using Microsoft.Extensions.Caching.Memory;

namespace Sorling.SqlConnAuthWeb.authentication;

//https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/Cookies/samples/CookieSessionSample/MemoryCacheTicketStore.cs
public class SqlAuthPwdMemoryStore : ISqlAuthPwdStore
{
   private const string _keyPrefix = $"{nameof(SqlAuthPwdMemoryStore)}-";
   private readonly MemoryCache _cache;

   public SqlAuthPwdMemoryStore() => _cache = new MemoryCache(new MemoryCacheOptions());

   public async Task<string> StoreAsync(SqlAuthStoredSecrets storedSecrets) {
      string? key = _keyPrefix + Guid.NewGuid().ToString();
      await RenewAsync(key, storedSecrets);
      return key;
   }

   public Task RenewAsync(string key, SqlAuthStoredSecrets storedSecrets) {
      _ = _cache.Set(key, storedSecrets, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(3) });
      return Task.FromResult(0);
   }

   public Task<SqlAuthStoredSecrets?> RetrieveAsync(string key) {
      _ = _cache.TryGetValue(key, out SqlAuthStoredSecrets? secrets);
      return Task.FromResult(secrets);
   }

   public Task RemoveAsync(string key) {
      _cache.Remove(key);
      return Task.FromResult(0);
   }
}

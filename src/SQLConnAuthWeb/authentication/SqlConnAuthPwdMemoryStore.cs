using Microsoft.Extensions.Caching.Memory;

namespace Sorling.SqlConnAuthWeb.authentication;

//https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/Cookies/samples/CookieSessionSample/MemoryCacheTicketStore.cs
public class SqlConnAuthPwdMemoryStore : ISqlConnAuthPwdStore
{
   private const string _keyPrefix = $"{nameof(SqlConnAuthPwdMemoryStore)}-";
   private readonly IMemoryCache _cache;

   public SqlConnAuthPwdMemoryStore() => _cache = new MemoryCache(new MemoryCacheOptions());

   public async Task<string> StoreAsync(string pwd) {
      string? key = _keyPrefix + Guid.NewGuid().ToString();
      await RenewAsync(key, pwd);
      return key;
   }

   public Task RenewAsync(string key, string pwd) {
      _ = _cache.Set(key, pwd, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(3) });
      return Task.FromResult(0);
   }

   public Task<string?> RetrieveAsync(string key) {
      _ = _cache.TryGetValue(key, out string? pwd);
      return Task.FromResult(pwd);
   }

   public Task RemoveAsync(string key) {
      _cache.Remove(key);
      return Task.FromResult(0);
   }
}

using Microsoft.Extensions.Caching.Memory;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// An in-memory implementation of <see cref="ISqlAuthPwdStore"/> for storing SQL authentication secrets using <see cref="MemoryCache"/>.
/// </summary>
public class SqlAuthPwdMemoryStore : ISqlAuthPwdStore
{
    private const string _keyPrefix = $"{nameof(SqlAuthPwdMemoryStore)}-";
    private readonly MemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlAuthPwdMemoryStore"/> class.
    /// </summary>
    public SqlAuthPwdMemoryStore() => _cache = new MemoryCache(new MemoryCacheOptions());

    /// <summary>
    /// Stores the provided SQL authentication secrets and returns a unique key for retrieval.
    /// </summary>
    /// <param name="storedSecrets">The secrets to store.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unique key for the stored secrets.</returns>
    public async Task<string> StoreAsync(SqlAuthStoredSecrets storedSecrets) {
        string? key = _keyPrefix + Guid.NewGuid().ToString();
        await RenewAsync(key, storedSecrets);
        return key;
    }

    /// <summary>
    /// Renews or updates the stored secrets associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the stored secrets to renew.</param>
    /// <param name="storedSecrets">The new secrets to store.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RenewAsync(string key, SqlAuthStoredSecrets storedSecrets) {
        _ = _cache.Set(key, storedSecrets, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(3) });
        return Task.FromResult(0);
    }

    /// <summary>
    /// Retrieves the stored secrets associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the stored secrets to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the stored secrets, or null if not found.</returns>
    public Task<SqlAuthStoredSecrets?> RetrieveAsync(string key) {
        _ = _cache.TryGetValue(key, out SqlAuthStoredSecrets? secrets);
        return Task.FromResult(secrets);
    }

    /// <summary>
    /// Removes the stored secrets associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the stored secrets to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RemoveAsync(string key) {
        _cache.Remove(key);
        return Task.FromResult(0);
    }
}

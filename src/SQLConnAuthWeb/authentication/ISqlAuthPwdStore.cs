namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Defines methods for securely storing, renewing, retrieving, and removing SQL authentication secrets.
/// </summary>
public interface ISqlAuthPwdStore
{
    /// <summary>
    /// Stores the provided SQL authentication secrets and returns a unique key for retrieval.
    /// </summary>
    /// <param name="storedSecrets">The secrets to store.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unique key for the stored secrets.</returns>
    public Task<string> StoreAsync(SqlAuthStoredSecrets storedSecrets);

    /// <summary>
    /// Renews or updates the stored secrets associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the stored secrets to renew.</param>
    /// <param name="storedSecrets">The new secrets to store.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RenewAsync(string key, SqlAuthStoredSecrets storedSecrets);

    /// <summary>
    /// Retrieves the stored secrets associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the stored secrets to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the stored secrets, or null if not found.</returns>
    public Task<SqlAuthStoredSecrets?> RetrieveAsync(string key);

    /// <summary>
    /// Removes the stored secrets associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the stored secrets to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RemoveAsync(string key);
}

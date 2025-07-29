namespace Sorling.SqlConnAuthWeb.authentication.passwords;

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

   /// <summary>
   /// Stores a temporary password and returns a unique key for retrieval. The implementation should use a cryptographically secure key.
   /// </summary>
   /// <param name="userName">The username associated with the password.</param>
   /// <param name="serverName">The server name associated with the password.</param>
   /// <param name="password">The password to store.</param>
   /// <param name="trustServerCertificate">Whether to trust the server certificate.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the generated key.</returns>
   public Task<string> SetTempPasswordAsync(string userName, string serverName, string password, bool trustServerCertificate);

   /// <summary>
   /// Retrieves and removes the temporary password info associated with the specified key.
   /// </summary>
   /// <param name="key">The unique key identifying the temporary password to retrieve.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the password info, or null if not found.</returns>
   public Task<SqlAuthTempPasswordInfo?> GetTempPasswordAsync(string key);

   /// <summary>
   /// Retrieves the temporary password info associated with the specified key without removing it.
   /// </summary>
   /// <param name="key">The unique key identifying the temporary password to peek.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the password info, or null if not found.</returns>
   public Task<SqlAuthTempPasswordInfo?> PeekTempPasswordAsync(string key);
}

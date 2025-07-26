using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication;
using System;
using System.Threading.Tasks;

namespace SQLConnAuthWebTests;

[TestClass]
public class SqlAuthPwdMemoryStoreTests
{
    [TestMethod]
    public async Task StoreAndRetrieveSecrets_WorksCorrectlyAsync()
    {
        SqlAuthPwdMemoryStore store = new();
        SqlAuthStoredSecrets secrets = new("pwd", false, null, "server");
        string key = await store.StoreAsync(secrets);
        SqlAuthStoredSecrets? retrieved = await store.RetrieveAsync(key);
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(secrets.Password, retrieved.Password);
    }

    [TestMethod]
    public async Task RemoveSecrets_RemovesSuccessfullyAsync()
    {
        SqlAuthPwdMemoryStore store = new();
        SqlAuthStoredSecrets secrets = new("pwd", false, null, "server");
        string key = await store.StoreAsync(secrets);
        await store.RemoveAsync(key);
        SqlAuthStoredSecrets? retrieved = await store.RetrieveAsync(key);
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public async Task SetTempPasswordAsync_And_GetTempPasswordAsync_WorksCorrectlyAsync()
    {
        SqlAuthPwdMemoryStore store = new();
        string key = await store.SetTempPasswordAsync("user", "server", "pwd", true);
        SqlAuthTempPasswordInfo? info = await store.GetTempPasswordAsync(key);
        Assert.IsNotNull(info);
        Assert.AreEqual("pwd", info.Password);
        Assert.IsTrue(info.TrustServerCertificate);
        // After retrieval, should be removed
        SqlAuthTempPasswordInfo? info2 = await store.GetTempPasswordAsync(key);
        Assert.IsNull(info2);
    }

    [TestMethod]
    public async Task PeekTempPasswordAsync_DoesNotRemoveAsync()
    {
        SqlAuthPwdMemoryStore store = new();
        string key = await store.SetTempPasswordAsync("user", "server", "pwd", false);
        SqlAuthTempPasswordInfo? info = await store.PeekTempPasswordAsync(key);
        Assert.IsNotNull(info);
        Assert.AreEqual("pwd", info.Password);
        Assert.IsFalse(info.TrustServerCertificate);
        // Should still exist after peek
        SqlAuthTempPasswordInfo? info2 = await store.PeekTempPasswordAsync(key);
        Assert.IsNotNull(info2);
    }

    [TestMethod]
    public void StoreAsync_NullSecrets_ThrowsExactlyArgumentNullException()
    {
        SqlAuthPwdMemoryStore store = new();
        _ = Assert.ThrowsExactly<NullReferenceException>(() => store.StoreAsync(null!).GetAwaiter().GetResult());
    }
}

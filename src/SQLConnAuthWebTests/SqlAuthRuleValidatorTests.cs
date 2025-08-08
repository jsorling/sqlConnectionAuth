using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.validation;
using System.Threading.Tasks;
using Moq;

namespace SQLConnAuthWebTests;

[TestClass]
public class SqlAuthRuleValidatorTests
{
   private static SqlAuthValidationRequest CreateRequest(string datasource, string password = "test", bool trustCert = false)
       => new(datasource, "user", password, trustCert);

   private static SqlAuthOptions DefaultOptions() => new() {
      AllowIntegratedSecurity = true,
      AllowLoopbackConnections = true,
      AllowPrivateNetworkConnections = true,
      AllowTrustServerCertificate = true
   };

   private static IOptionsMonitor<SqlAuthOptions> Monitor(SqlAuthOptions options)
   {
      Mock<IOptionsMonitor<SqlAuthOptions>> mock = new();
      _ = mock.Setup(m => m.CurrentValue).Returns(options);
      return mock.Object;
   }

   [TestMethod]
   public async Task ValidateAsync_WindowsAuthBlocked_ThrowsAsync() {
      SqlAuthOptions options = new() { AllowIntegratedSecurity = false };
      SqlAuthRuleValidator validator = new(Monitor(options));
      SqlAuthValidationRequest req = CreateRequest("127.0.0.1", SqlAuthConsts.WINDOWSAUTHENTICATION);
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNotNull(result.Exception);
      Assert.Contains("Windows authentication not allowed", result.Exception.Message);
   }

   [TestMethod]
   public async Task ValidateAsync_ResolveFails_ThrowsAsync() {
      SqlAuthOptions options = DefaultOptions();
      SqlAuthRuleValidator validator = new(Monitor(options));
      SqlAuthValidationRequest req = CreateRequest("not.a.real.host");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNotNull(result.Exception);
      Assert.Contains("Unable to resolve SQL Server address", result.Exception.Message);
   }

   [TestMethod]
   public async Task ValidateAsync_NoIpFound_ThrowsAsync() {
      SqlAuthOptions options = DefaultOptions();
      SqlAuthRuleValidator validator = new(Monitor(options));
      // Use a reserved, non-routable address that will not resolve to any IP
      SqlAuthValidationRequest req = CreateRequest("0.0.0.0");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNotNull(result.Exception);
      Assert.Contains("No IP address found", result.Exception.Message);
   }

   [TestMethod]
   public async Task ValidateAsync_LoopbackBlocked_ThrowsAsync() {
      SqlAuthOptions options = DefaultOptions();
      options.AllowLoopbackConnections = false;
      SqlAuthRuleValidator validator = new(Monitor(options));
      SqlAuthValidationRequest req = CreateRequest("127.0.0.1");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNotNull(result.Exception);
      Assert.Contains("Loopback connections not allowed", result.Exception.Message);
   }

   [TestMethod]
   public async Task ValidateAsync_PrivateBlocked_ThrowsAsync() {
      SqlAuthOptions options = DefaultOptions();
      options.AllowPrivateNetworkConnections = false;
      SqlAuthRuleValidator validator = new(Monitor(options));
      SqlAuthValidationRequest req = CreateRequest("192.168.1.1");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNotNull(result.Exception);
      Assert.Contains("Private network connections not allowed", result.Exception.Message);
   }

   [TestMethod]
   public async Task ValidateAsync_AllowedIpList_AllowsMatchAsync() {
      SqlAuthOptions options = DefaultOptions();
      options.AllowedIPAddresses.Add("127.0.0.1");
      SqlAuthRuleValidator validator = new(Monitor(options));
      SqlAuthValidationRequest req = CreateRequest("127.0.0.1");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNull(result.Exception);
      Assert.IsNotNull(result.StoredSecrets);
   }

   [TestMethod]
   public async Task ValidateAsync_AllowedIpList_BlocksNonMatchAsync() {
      SqlAuthOptions options = DefaultOptions();
      options.AllowedIPAddresses.Add("10.0.0.1");
      SqlAuthRuleValidator validator = new(Monitor(options));
      SqlAuthValidationRequest req = CreateRequest("127.0.0.1");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNotNull(result.Exception);
      Assert.Contains("IP address not allowed by allow-list", result.Exception.Message);
   }

   [TestMethod]
   public async Task ValidateAsync_AllowedIpList_AllowsRangeMatchAsync() {
      SqlAuthOptions options = DefaultOptions();
      options.AllowedIPAddresses.Add("192.168.1.0/24");
      SqlAuthRuleValidator validator = new(Monitor(options));
      // Use a hostname that resolves to an IP in the 192.168.1.0/24 range, or use a direct IP
      SqlAuthValidationRequest req = CreateRequest("192.168.1.42");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNull(result.Exception);
      Assert.IsNotNull(result.StoredSecrets);
   }

   [TestMethod]
   public async Task ValidateAsync_AllowedIpList_BlocksRangeNonMatchAsync() {
      SqlAuthOptions options = DefaultOptions();
      options.AllowedIPAddresses.Add("192.168.1.0/24");
      SqlAuthRuleValidator validator = new(Monitor(options));
      SqlAuthValidationRequest req = CreateRequest("192.168.2.42");
      SqlAuthRuleValidationResult result = await validator.ValidateAsync(req);
      Assert.IsNotNull(result.Exception);
      Assert.Contains("IP address not allowed by allow-list", result.Exception.Message);
   }
}

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication;
using System;
using System.IO;

namespace SQLConnAuthWebTests;

[TestClass]
public class TestsInitialize
{
   private static SqlConnAuthenticationData? _sqlConnAuthenticationData;

   public static SqlConnAuthenticationData SQLConnAuthenticationData
      => _sqlConnAuthenticationData ?? throw new ApplicationException("Failed to load SQLConnAuthenticationData-configuration");

   [AssemblyInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
   public static void AssemblyInitialize(TestContext testContext) {
#pragma warning restore IDE0060 // Remove unused parameter
      IConfigurationSection conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", true, false).AddUserSecrets<TestsInitialize>().Build().GetSection("SQLConnAuthenticationData");

      _sqlConnAuthenticationData = new(conf["SqlServer"], conf["UserName"], conf["Password"]);
   }
}

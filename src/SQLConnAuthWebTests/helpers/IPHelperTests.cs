using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.helpers;
using System.Net;
using System.Threading.Tasks;

namespace SQLConnAuthWebTests.helpers;
[TestClass]
public class IPHelperTests
{
   [TestMethod]
   public void GetSQLServerAddress_ValidDataSource_ReturnsCorrectAddress() {
      string[] datasource = [ "serverName\\instanceName,1433"
            , "serverName\\instanceName"
            , "serverName,1433"
            , "serverName"
            , "serverName\\"
            , "serverName,"
            , "serverName , 1433"
         ];

      string expectedaddress = "serverName";

      foreach (string data in datasource)
      {
         string result = IPHelper.GetSQLServerAddress(data);
         Assert.AreEqual(expectedaddress, result);
      }
   }

   [TestMethod]
   public void GetSQLServerInstance_ValidDataSource_ReturnsCorrectInstance() {
      string[] datasource = [ "serverName\\instanceName,1433"
            , "serverName\\instanceName"
            , ".\\instanceName,1433"
            , ".\\instanceName , 1433"
         ];

      string expectedinstance = "instanceName";

      foreach (string data in datasource)
      {
         string result = IPHelper.GetSQLServerInstance(data);
         Assert.AreEqual(expectedinstance, result);
      }
   }

   [TestMethod]
   public void GetSQLServerPort_ValidDataSource_ReturnsCorrectPort() {
      string[] datasource = [ "serverName\\instanceName,1433"
            , "serverName\\instanceName"
            , "serverName,1433"
            , "serverName"
            , "serverName\\"
            , "serverName,"
            , "serverName , 1433"
         ];

      int expectedport = 1433;

      foreach (string data in datasource)
      {
         int result = IPHelper.GetSQLServerPort(data);
         Assert.AreEqual(expectedport, result);
      }

      datasource = [ "serverName\\instanceName,2012"
            , "serverName,2012"
            , "serverName , 2012"
         ];

      expectedport = 2012;

      foreach (string data in datasource)
      {
         int result = IPHelper.GetSQLServerPort(data);
         Assert.AreEqual(expectedport, result);
      }
   }

   [TestMethod]
   public async Task ResolveSqlIPAddressAsync_ValidDataSource_ReturnsCorrectIPAddressAsync() {
      string[] datasource = [ ".\\instanceName,1433"
         , "localhost\\instanceName"
         , "dns.google.com,1433"
         , "192.168.1.233"
         , "127.0.0.1\\"
      ];

      foreach (string data in datasource)
      {
         IPAddress[] result = await IPHelper.ResolveSqlIPAddressAsync(data);

         Assert.IsTrue(result.Length > 0);
      }
   }

   [TestMethod]
   public void GetIPNetworkType_ValidDataSource_ReturnsCorrectIPNetworkType() {
      string[] datasource = [".\\instanceName,1433"
         , "localhost\\instanceName"
         , "127.0.0.1"
         , "::1"
         , "::1\\instanceName"
      ];

      foreach (string data in datasource)
      {
         IPAddress[] result = IPHelper.ResolveSqlIPAddressAsync(data).Result;
         Assert.IsTrue(result.Length > 0);

         foreach (IPAddress ip in result)
         {
            IPNetworkType type = IPReservedNetworks.GetIPNetworkType(ip);
            Assert.AreEqual(IPNetworkType.Loopback, type);
         }
      }

      datasource = ["192.168.1.12\\instanceName,1433"
         , "10.0.0.3\\instanceName"
         , "100.64.0.12"
         , "172.16.1.2"
         , "fc01::"
         , "fc01::\\instanceName"
      ];

      foreach (string data in datasource)
      {
         IPAddress[] result = IPHelper.ResolveSqlIPAddressAsync(data).Result;
         Assert.IsTrue(result.Length > 0);

         foreach (IPAddress ip in result)
         {
            IPNetworkType type = IPReservedNetworks.GetIPNetworkType(ip);
            Assert.AreEqual(IPNetworkType.Private, type);
         }
      }

      datasource = ["svt.se\\instanceName,1433"
         , "google.com\\instanceName"
         , "microsoft.com"
         , "dn.se"
      ];

      foreach (string data in datasource)
      {
         IPAddress[] result = IPHelper.ResolveSqlIPAddressAsync(data).Result;
         Assert.IsTrue(result.Length > 0);

         foreach (IPAddress ip in result)
         {
            IPNetworkType type = IPReservedNetworks.GetIPNetworkType(ip);
            Assert.AreEqual(IPNetworkType.Public, type);
         }
      }
   }
}

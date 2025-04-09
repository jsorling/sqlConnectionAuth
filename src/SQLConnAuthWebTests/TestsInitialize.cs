using Microsoft.Extensions.Configuration;
using Sorling.SqlConnAuthWeb.authentication;
using System.IO;
using System.Reflection;

namespace SQLConnAuthWebTests;

public static class TestsInitialize
{
   private static readonly IConfigurationSection _conf = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", true, false)
      .AddUserSecrets(Assembly.GetExecutingAssembly())
      .Build().GetSection("SQLConnAuthenticationData");

   public static SqlConnAuthenticationData SQLConnAuthenticationData(string? sqlServer = null
      , string? userName = null, string? password = null) {
      SqlConnAuthenticationData tor = new(sqlServer ?? _conf["SqlServer"]
         , userName ?? _conf["UserName"]
         , new(password ?? _conf["Password"] ?? "", true));

      return tor;
   }
}

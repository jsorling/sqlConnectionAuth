using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;
using Sorling.SqlConnAuthWeb.helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLConnAuthWebTests.helpers;

[TestClass]
public class SqlConnectionHelperTests
{
   [TestMethod]
   public void GetVersion() {
      string? t = SqlConnectionHelper.GetSqlServerVersionAsync(TestsInitialize.SQLConnAuthenticationData()).Result;

      Console.WriteLine(t);

      Assert.IsFalse(string.IsNullOrEmpty(t));
   }

   [TestMethod]
   public void ListDbs() {
      IEnumerable<ISqlDatabase> t
         = SqlConnectionHelper.GetDatabasesAsync(TestsInitialize.SQLConnAuthenticationData()).Result;

      Console.WriteLine(t.First().Name);

      Assert.IsTrue(t.Any());
   }

   [TestMethod]
   public void WrongServerVersion() {
      AggregateException ex = Assert.ThrowsExactly<AggregateException>(
         () => _ = _ = SqlConnectionHelper.GetSqlServerVersionAsync(TestsInitialize.SQLConnAuthenticationData(sqlServer: "xx")).Result);

      Console.WriteLine(ex.Message);
   }

   [TestMethod]
   public void WrongUserNameVersion() {
      AggregateException ex = Assert.ThrowsExactly<AggregateException>(
         () => _ = _ = SqlConnectionHelper.GetSqlServerVersionAsync(TestsInitialize.SQLConnAuthenticationData(userName: "xx")).Result);

      Console.WriteLine(ex.Message);
   }

   [TestMethod]
   public void WrongPasswordVersion() {
      AggregateException ex = Assert.ThrowsExactly<AggregateException>(
         () => _ = _ = SqlConnectionHelper.GetSqlServerVersionAsync(TestsInitialize.SQLConnAuthenticationData(password: "xx")).Result);

      Console.WriteLine(ex.Message);
   }
}

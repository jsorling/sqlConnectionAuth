using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLConnAuthWebTests;

[TestClass]
public class SqlConnectionHelperTests
{
   [TestMethod]
   public void GetVersion() {
      string? t = SqlConnectionHelper.GetVerionAsync(TestsInitialize.SQLConnAuthenticationData()).Result;

      Console.WriteLine(t);

      Assert.IsFalse(string.IsNullOrEmpty(t));
   }

   [TestMethod]
   public void ListDbs() {
      IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes> t 
         = SqlConnectionHelper.GetDbsAsync(TestsInitialize.SQLConnAuthenticationData()).Result;

      Console.WriteLine(t.First().Name);

      Assert.IsTrue(t.Any());
   }

   [TestMethod]
   public void WrongServerVersion() {
      AggregateException ex = Assert.ThrowsExactly<AggregateException>(
         () => _ = _ = SqlConnectionHelper.GetVerionAsync(TestsInitialize.SQLConnAuthenticationData(sqlServer:"xx")).Result);

      Console.WriteLine(ex.Message);
   }  

   [TestMethod]
   public void WrongUserNameVersion() {
      AggregateException ex = Assert.ThrowsExactly<AggregateException>(
         () => _ = _ = SqlConnectionHelper.GetVerionAsync(TestsInitialize.SQLConnAuthenticationData(userName: "xx")).Result);

      Console.WriteLine(ex.Message);
   }

   [TestMethod]
   public void WrongPasswordVersion() {
      AggregateException ex = Assert.ThrowsExactly<AggregateException>(
         () => _ = _ = SqlConnectionHelper.GetVerionAsync(TestsInitialize.SQLConnAuthenticationData(password: "xx")).Result);

      Console.WriteLine(ex.Message);
   }
}

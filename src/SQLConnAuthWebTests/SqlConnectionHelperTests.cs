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
   public void SQLConnAuthenticationDataTest() {
      Console.WriteLine(TestsInitialize.SQLConnAuthenticationData.SqlServer);
      Assert.IsTrue(!string.IsNullOrEmpty(TestsInitialize.SQLConnAuthenticationData.SqlServer));
   }

   [TestMethod]
   public void GetVersion() {
      string? t = SqlConnectionHelper.GetVerionAsync(TestsInitialize.SQLConnAuthenticationData).Result;

      Console.WriteLine(t);

      Assert.IsTrue(!string.IsNullOrEmpty(t));
   }

   [TestMethod]
   public void ListDbs() {
      IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes> t = SqlConnectionHelper.GetDbsAsync(TestsInitialize.SQLConnAuthenticationData).Result;

      Console.WriteLine(t.First().Name);

      Assert.IsTrue(t.Any());
   }

   [TestMethod]
   public void WrongServerVersion() {
      SqlConnAuthenticationData wsca = new(TestsInitialize.SQLConnAuthenticationData.SqlServer! + "x"
         , TestsInitialize.SQLConnAuthenticationData.UserName
         , TestsInitialize.SQLConnAuthenticationData.Password!);
      AggregateException ex = Assert.ThrowsException<AggregateException>(
         () => _ = SqlConnectionHelper.GetVerionAsync(wsca).Result);

      Console.WriteLine(ex.Message);
   }

   //[TestMethod]
   //public void WrongDBVersion() {
   //   AggregateException ex = Assert.ThrowsException<AggregateException>(
   //      () => _ = SQLConnectionHelper.GetVerionAsync(TestsInitialize.SQLConnAuthenticationData.SqlServer!
   //      , TestsInitialize.SQLConnAuthenticationData.SqlDb + "wrong"
   //      , TestsInitialize.SQLConnAuthenticationData.UserName!
   //      , TestsInitialize.SQLConnAuthenticationData.Password!).Result);

   //   Console.WriteLine(ex.Message);
   //}

   [TestMethod]
   public void WrongUserNameVersion() {
      SqlConnAuthenticationData wsca = new(TestsInitialize.SQLConnAuthenticationData.SqlServer!
         , TestsInitialize.SQLConnAuthenticationData.UserName + "x"
         , TestsInitialize.SQLConnAuthenticationData.Password!);
      AggregateException ex = Assert.ThrowsException<AggregateException>(
         () => _ = SqlConnectionHelper.GetVerionAsync(wsca).Result);

      Console.WriteLine(ex.Message);
   }

   [TestMethod]
   public void WrongPasswordVersion() {
      SqlConnAuthenticationData wsca = new(TestsInitialize.SQLConnAuthenticationData.SqlServer!
         , TestsInitialize.SQLConnAuthenticationData.UserName
         , TestsInitialize.SQLConnAuthenticationData.Password! + "x");
      AggregateException ex = Assert.ThrowsException<AggregateException>(
         () => _ = SqlConnectionHelper.GetVerionAsync(wsca).Result);

      Console.WriteLine(ex.Message);
   }
}

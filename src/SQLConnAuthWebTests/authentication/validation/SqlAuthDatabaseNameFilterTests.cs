using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.validation;
using System;
using System.Collections.Generic;

namespace SQLConnAuthWebTests.authentication.validation;

[TestClass]
public class SqlAuthDatabaseNameFilterTests
{
   private static IOptionsMonitor<SqlAuthOptions> CreateOptions(
       IEnumerable<string>? allow = null,
       IEnumerable<string>? deny = null) {
      SqlAuthOptions options = new() {
         IncludeDatabaseFilter = allow != null ? [.. allow] : [],
         ExcludeDatabaseFilter = deny != null ? [.. deny] : []
      };
      return new TestOptionsMonitor(options);
   }

   private class TestOptionsMonitor(SqlAuthOptions value) : IOptionsMonitor<SqlAuthOptions>
   {
      public SqlAuthOptions CurrentValue { get; } = value;
      public SqlAuthOptions Get(string? name) => CurrentValue;
      public IDisposable OnChange(System.Action<SqlAuthOptions, string?> listener) => new DummyDisposable();
      private class DummyDisposable : IDisposable { public void Dispose() { } }
   }

   [TestMethod]
   public void IsAllowed_DenyTakesPrecedence() {
      SqlAuthDatabaseNameFilter validator = new(CreateOptions(
            allow: ["*"], deny: ["SecretDB"]));
      Assert.IsFalse(validator.IsAllowed("SecretDB"));
      Assert.IsTrue(validator.IsAllowed("OtherDB"));
   }

   [TestMethod]
   public void IsAllowed_AllowAllIfAllowEmpty() {
      SqlAuthDatabaseNameFilter validator = new(CreateOptions(allow: null, deny: null));
      Assert.IsTrue(validator.IsAllowed("AnyDB"));
   }

   [TestMethod]
   public void IsAllowed_AllowPatternWithWildcard() {
      SqlAuthDatabaseNameFilter validator = new(CreateOptions(allow: ["Test*"]));
      Assert.IsTrue(validator.IsAllowed("TestDB"));
      Assert.IsFalse(validator.IsAllowed("ProdDB"));
   }

   [TestMethod]
   public void IsAllowed_DenyPatternWithWildcard() {
      SqlAuthDatabaseNameFilter validator = new(CreateOptions(allow: ["*"], deny: ["Prod*"]));
      Assert.IsFalse(validator.IsAllowed("ProdDB"));
      Assert.IsTrue(validator.IsAllowed("DevDB"));
   }

   [TestMethod]
   public void ListAllowed_FiltersCorrectly() {
      SqlAuthDatabaseNameFilter validator = new(CreateOptions(allow: ["A*"], deny: ["Admin"]));
      string[] input = ["Admin", "Alpha", "Beta", "Apple"];
      string[] allowed = [.. validator.ListAllowed(input)];
      CollectionAssert.AreEquivalent(new[] { "Alpha", "Apple" }, allowed);
   }

   [TestMethod]
   public void IsAllowed_NullOrWhitespace_ReturnsFalse() {
      SqlAuthDatabaseNameFilter validator = new(CreateOptions());
      Assert.IsFalse(validator.IsAllowed(null!));
      Assert.IsFalse(validator.IsAllowed(" "));
   }
}

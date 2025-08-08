using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLConnAuthWebTests.authentication.validation;

[TestClass]
public class SqlAuthDatabaseNameValidatorTests
{
   private static IOptionsMonitor<SqlAuthOptions> CreateOptions(
       IEnumerable<string>? allow = null,
       IEnumerable<string>? deny = null) {
      SqlAuthOptions options = new() {
         AllowDatabases = allow != null ? [.. allow] : [],
         DenyDatabases = deny != null ? [.. deny] : []
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
      SqlAuthDatabaseNameValidator validator = new(CreateOptions(
            allow: ["*"], deny: ["SecretDB"]));
      Assert.IsFalse(validator.IsAllowed("SecretDB"));
      Assert.IsTrue(validator.IsAllowed("OtherDB"));
   }

   [TestMethod]
   public void IsAllowed_AllowAllIfAllowEmpty() {
      SqlAuthDatabaseNameValidator validator = new(CreateOptions(allow: null, deny: null));
      Assert.IsTrue(validator.IsAllowed("AnyDB"));
   }

   [TestMethod]
   public void IsAllowed_AllowPatternWithWildcard() {
      SqlAuthDatabaseNameValidator validator = new(CreateOptions(allow: new[] { "Test*" }));
      Assert.IsTrue(validator.IsAllowed("TestDB"));
      Assert.IsFalse(validator.IsAllowed("ProdDB"));
   }

   [TestMethod]
   public void IsAllowed_DenyPatternWithWildcard() {
      SqlAuthDatabaseNameValidator validator = new(CreateOptions(allow: new[] { "*" }, deny: new[] { "Prod*" }));
      Assert.IsFalse(validator.IsAllowed("ProdDB"));
      Assert.IsTrue(validator.IsAllowed("DevDB"));
   }

   [TestMethod]
   public void ListAllowed_FiltersCorrectly() {
      SqlAuthDatabaseNameValidator validator = new(CreateOptions(allow: new[] { "A*" }, deny: new[] { "Admin" }));
      string[] input = ["Admin", "Alpha", "Beta", "Apple"];
      string[] allowed = validator.ListAllowed(input).ToArray();
      CollectionAssert.AreEquivalent(new[] { "Alpha", "Apple" }, allowed);
   }

   [TestMethod]
   public void IsAllowed_NullOrWhitespace_ReturnsFalse() {
      SqlAuthDatabaseNameValidator validator = new(CreateOptions());
      Assert.IsFalse(validator.IsAllowed(null!));
      Assert.IsFalse(validator.IsAllowed(" "));
   }
}

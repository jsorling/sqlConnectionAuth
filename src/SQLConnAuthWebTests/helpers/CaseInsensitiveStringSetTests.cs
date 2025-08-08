using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.helpers;
using System.Collections.Generic;

namespace SQLConnAuthWebTests.helpers;

[TestClass]
public class CaseInsensitiveStringSetTests
{
   [TestMethod]
   public void Add_AddsUniqueItems_CaseInsensitive() {
      CaseInsensitiveStringSet set = [];
      Assert.IsTrue(set.Add("Test"));
      Assert.IsFalse(set.Add("test")); // duplicate, different case
      Assert.AreEqual(1, set.Count);
   }

   [TestMethod]
   public void Remove_RemovesItem_CaseInsensitive() {
      CaseInsensitiveStringSet set = ["Alpha", "Beta"];
      Assert.IsTrue(set.Remove("alpha"));
      Assert.IsFalse(set.Contains("ALPHA"));
      Assert.AreEqual(1, set.Count);
   }

   [TestMethod]
   public void Contains_IsCaseInsensitive() {
      CaseInsensitiveStringSet set = ["Hello"];
      Assert.IsTrue(set.Contains("hello"));
      Assert.IsTrue(set.Contains("HELLO"));
      Assert.IsFalse(set.Contains("world"));
   }

   [TestMethod]
   public void Clear_RemovesAllItems() {
      CaseInsensitiveStringSet set = ["A", "B"];
      set.Clear();
      Assert.AreEqual(0, set.Count);
   }

   [TestMethod]
   public void ToArray_ReturnsAllItems() {
      string[] items = ["One", "Two"];
      CaseInsensitiveStringSet set = [.. items];
      string[] arr = [.. set];
      CollectionAssert.AreEquivalent(items, arr);
   }

   [TestMethod]
   public void Enumerator_EnumeratesAllItems() {
      string[] items = ["X", "Y"];
      CaseInsensitiveStringSet set = [.. items];
      List<string> list = [.. set];
      CollectionAssert.AreEquivalent(items, list);
   }
}

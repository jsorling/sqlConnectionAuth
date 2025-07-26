using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.extenstions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLConnAuthWebTests;

[TestClass]
public class StringFilterExtensionsTests
{
   private static readonly string[] _matchAsteriskWildcard = ["apple"];

   [TestMethod]
   public void FilterWithSearchPattern_MatchAsteriskWildcard_ReturnsMatching() {
      string[] input = ["apple", "apricot", "banana", "grape"];
      List<string> result = [.. input.FilterWithSearchPattern("a*e")];
      CollectionAssert.AreEquivalent(_matchAsteriskWildcard, result);
   }

   private static readonly string[] _matchUnderscoreWildcard = ["cat", "cot", "cut"];

   [TestMethod]
   public void FilterWithSearchPattern_MatchUnderscoreWildcard_ReturnsMatching() {
      string[] input = ["cat", "cot", "cut", "cart"];
      string pattern = "c_t";
      List<string> result = [.. input.FilterWithSearchPattern(pattern)];
      // Debug output
      Console.WriteLine($"Pattern: {pattern}");
      Console.WriteLine($"Results: {string.Join(", ", result)}");
      CollectionAssert.AreEquivalent(_matchUnderscoreWildcard, result);
   }

   private static readonly string[] _caseSensitive = ["Dog"];

   [TestMethod]
   public void FilterWithSearchPattern_CaseSensitive_WorksCorrectly() {
      string[] input = ["Dog", "dog", "DOG"];
      List<string> result = [.. input.FilterWithSearchPattern("Dog", caseSensitive: true)];
      CollectionAssert.AreEquivalent(_caseSensitive, result);
   }

   [TestMethod]
   public void FilterWithSearchPattern_Negative_WorksCorrectly() {
      string[] input = ["red", "green", "blue"];
      List<string> result = [.. input.FilterWithSearchPattern("*e*", negative: true)];
      // All items contain 'e', so negative should return an empty list
      CollectionAssert.AreEquivalent(Array.Empty<string>(), result);
   }

   [TestMethod]
   public void FilterWithSearchPattern_NullSource_ThrowsArgumentNullException() {
      IEnumerable<string> input = null!;
      _ = Assert.ThrowsExactly<ArgumentNullException>(() => input.FilterWithSearchPattern("*").ToList());
   }

   [TestMethod]
   public void FilterWithSearchPattern_NullPattern_ThrowsArgumentNullException() {
      string[] input = ["a", "b"];
      _ = Assert.ThrowsExactly<ArgumentNullException>(() => input.FilterWithSearchPattern(null!).ToList());
   }

   private static readonly string[] _exactMatch_NoWildcards_CaseInsensitive = ["Test", "test", "TEST"];

   [TestMethod]
   public void FilterWithSearchPattern_ExactMatch_NoWildcards_CaseInsensitive() {
      string[] input = ["Test", "test", "TEST", "toast"];
      List<string> result = [.. input.FilterWithSearchPattern("test")];
      CollectionAssert.AreEquivalent(_exactMatch_NoWildcards_CaseInsensitive, result);
   }

   private static readonly string[] _exactMatch_NoWildcards_CaseSensitive = ["test"];

   [TestMethod]
   public void FilterWithSearchPattern_ExactMatch_NoWildcards_CaseSensitive() {
      string[] input = ["Test", "test", "TEST", "toast"];
      List<string> result = [.. input.FilterWithSearchPattern("test", caseSensitive: true)];
      CollectionAssert.AreEquivalent(_exactMatch_NoWildcards_CaseSensitive, result);
   }
}

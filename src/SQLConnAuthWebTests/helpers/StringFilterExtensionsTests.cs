using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorling.SqlConnAuthWeb.extenstions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLConnAuthWebTests.helpers;

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

   [TestMethod]
   public void FilterWithSearchPatterns_MatchAnyPattern_ReturnsMatching() {
      string[] input = ["apple", "apricot", "banana", "grape", "cat", "cot", "cut", "cart"];
      string[] patterns = ["a*e", "c_t"];
      List<string> result = [.. input.FilterWithSearchPatterns(patterns)];
      string[] expected = ["apple", "cat", "cot", "cut"];
      CollectionAssert.AreEquivalent(expected, result);
   }

   [TestMethod]
   public void FilterWithSearchPatterns_Negative_ReturnsNonMatching() {
      string[] input = ["apple", "apricot", "banana", "grape", "cat", "cot", "cut", "cart"];
      string[] patterns = ["a*e", "c_t"];
      List<string> result = [.. input.FilterWithSearchPatterns(patterns, negative: true)];
      string[] expected = ["apricot", "banana", "grape", "cart"];
      CollectionAssert.AreEquivalent(expected, result);
   }

   [TestMethod]
   public void FilterWithSearchPatterns_EmptyPatterns_ReturnsAll() {
      string[] input = ["a", "b", "c"];
      string[] patterns = [];
      List<string> result = [.. input.FilterWithSearchPatterns(patterns)];
      CollectionAssert.AreEquivalent(input, result);
   }

   [TestMethod]
   public void FilterWithSearchPatterns_NullPatterns_ThrowsArgumentNullException() {
      string[] input = ["a", "b"];
      string[] patterns = null!;
      _ = Assert.ThrowsExactly<ArgumentNullException>(() => input.FilterWithSearchPatterns(patterns).ToList());
   }

   private static readonly string[] _expected = ["", "apple"];

   [TestMethod]
   public void FilterWithSearchPattern_MatchEmptyString_ReturnsMatching() {
      string[] input = [null!, "", "apple"];
      List<string> result = [.. input.FilterWithSearchPattern("*")];
      // "" and "apple" should match
      CollectionAssert.AreEquivalent(_expected, result);
   }

   [TestMethod]
   public void FilterWithSearchPatterns_MatchEmptyString_ReturnsMatching() {
      string[] input = [null!, "", "apple"];
      string[] patterns = ["*"];
      List<string> result = [.. input.FilterWithSearchPatterns(patterns)];
      CollectionAssert.AreEquivalent(_expected, result);
   }
}

using System.Text.RegularExpressions;

namespace Sorling.SqlConnAuthWeb.extenstions;

/// <summary>
/// Provides extension methods for filtering strings in collections using wildcard patterns.
/// </summary>
public static class StringFilterExtensions
{
   /// <summary>
   /// Filters a collection of strings based on a search pattern with wildcards.
   /// Delegates to <see cref="FilterWithSearchPatterns(IEnumerable{string}, IEnumerable{string}, bool, bool)"/>.
   /// Null checks are performed in the plural-pattern method.
   /// </summary>
   /// <param name="source">The collection of strings to filter.</param>
   /// <param name="pattern">The search pattern, where '*' matches any sequence of characters and '_' matches any single character.</param>
   /// <param name="caseSensitive">If true, the match is case sensitive; otherwise, it is case insensitive.</param>
   /// <param name="negative">If true, returns strings that do not match the pattern; otherwise, returns strings that match.</param>
   /// <returns>An enumerable of filtered strings based on the pattern and options.</returns>
   public static IEnumerable<string> FilterWithSearchPattern(this IEnumerable<string> source, string pattern
      , bool caseSensitive = false, bool negative = false) =>
      source.FilterWithSearchPatterns([pattern], caseSensitive, negative);

   private static bool IsMatchNonNull(string? s, Regex regex) => s is not null && regex.IsMatch(s);

   /// <summary>
   /// Filters a collection of any type based on a search pattern with wildcards, using a selector to extract the string to match.
   /// Delegates to <see cref="FilterWithSearchPatterns{T}(IEnumerable{T}, Func{T, string}, IEnumerable{string}, bool, bool)"/>.
   /// Null checks are performed in the plural-pattern method.
   /// </summary>
   /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
   /// <param name="source">The collection to filter.</param>
   /// <param name="selector">A function to extract the string to match from each element.</param>
   /// <param name="pattern">The search pattern, where '*' matches any sequence of characters and '_' matches any single character.</param>
   /// <param name="caseSensitive">If true, the match is case sensitive; otherwise, it is case insensitive.</param>
   /// <param name="negative">If true, returns elements that do not match the pattern; otherwise, returns elements that match.</param>
   /// <returns>An enumerable of filtered elements based on the pattern and options.</returns>
   public static IEnumerable<T> FilterWithSearchPattern<T>(this IEnumerable<T> source, Func<T, string> selector, string pattern
      , bool caseSensitive = false, bool negative = false) =>
      source.FilterWithSearchPatterns(selector, [pattern], caseSensitive, negative);

   /// <summary>
   /// Filters a collection of strings based on multiple search patterns with wildcards.
   /// Delegates to the generic plural-pattern method using the identity selector.
   /// </summary>
   /// <param name="source">The collection of strings to filter.</param>
   /// <param name="patterns">The search patterns, where '*' matches any sequence of characters and '_' matches any single character.</param>
   /// <param name="caseSensitive">If true, the match is case sensitive; otherwise, it is case insensitive.</param>
   /// <param name="negative">If true, returns strings that do not match any pattern; otherwise, returns strings that match at least one pattern.</param>
   /// <returns>An enumerable of filtered strings based on the patterns and options.</returns>
   public static IEnumerable<string> FilterWithSearchPatterns(this IEnumerable<string> source, IEnumerable<string> patterns
      , bool caseSensitive = false, bool negative = false) =>
      source.FilterWithSearchPatterns(s => s, patterns, caseSensitive, negative);

   /// <summary>
   /// Filters a collection of any type based on multiple search patterns with wildcards, using a selector to extract the string to match.
   /// This method performs all null checks and contains the core filtering logic.
   /// </summary>
   /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
   /// <param name="source">The collection to filter. Must not be null.</param>
   /// <param name="selector">A function to extract the string to match from each element. Must not be null.</param>
   /// <param name="patterns">The search patterns, where '*' matches any sequence of characters and '_' matches any single character. Must not be null.</param>
   /// <param name="caseSensitive">If true, the match is case sensitive; otherwise, it is case insensitive.</param>
   /// <param name="negative">If true, returns elements that do not match any pattern; otherwise, returns elements that match at least one pattern.</param>
   /// <returns>An enumerable of filtered elements based on the patterns and options. If <paramref name="patterns"/> is empty, returns the original <paramref name="source"/>.</returns>
   public static IEnumerable<T> FilterWithSearchPatterns<T>(this IEnumerable<T> source, Func<T, string> selector, IEnumerable<string> patterns
      , bool caseSensitive = false, bool negative = false) {
      ArgumentNullException.ThrowIfNull(source);
      ArgumentNullException.ThrowIfNull(selector);
      ArgumentNullException.ThrowIfNull(patterns);
      string[] patternlist = [.. patterns];
      if (patternlist.Length == 0)
         return source;
      Regex[] regexes = [.. patternlist.Select(pattern =>
         new Regex("^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("_", ".") + "$",
            caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))];
      return negative
         ? source.Where(item => !regexes.Any(r => IsMatchNonNull(selector(item), r)))
         : source.Where(item => regexes.Any(r => IsMatchNonNull(selector(item), r)));
   }
}

using System.Text.RegularExpressions;

namespace Sorling.SqlConnAuthWeb.extenstions;

/// <summary>
/// Provides extension methods for filtering strings in collections using wildcard patterns.
/// </summary>
public static class StringFilterExtensions
{
   /// <summary>
   /// Filters a collection of strings based on a search pattern with wildcards.
   /// </summary>
   /// <param name="source">The collection of strings to filter.</param>
   /// <param name="pattern">The search pattern, where '*' matches any sequence of characters and '_' matches any single character.</param>
   /// <param name="caseSensitive">If true, the match is case sensitive; otherwise, it is case insensitive.</param>
   /// <param name="negative">If true, returns strings that do not match the pattern; otherwise, returns strings that match.</param>
   /// <returns>An enumerable of filtered strings based on the pattern and options.</returns>
   public static IEnumerable<string> FilterWithSearchPattern(
       this IEnumerable<string> source,
       string pattern,
       bool caseSensitive = false,
       bool negative = false) {
      ArgumentNullException.ThrowIfNull(source);
      ArgumentNullException.ThrowIfNull(pattern);

      // Escape the pattern first, then replace wildcards
      string regexpattern = "^" + Regex.Escape(pattern)
            .Replace("\\*", ".*") // Escaped *
            .Replace("_", ".")     // Unescaped _
            + "$";
      RegexOptions options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
      Regex regex = new(regexpattern, options);

      // Debug output for diagnosis
      Console.WriteLine($"Regex pattern: {regexpattern}");
      foreach (string s in source)
         Console.WriteLine($"Testing '{s}' => {regex.IsMatch(s)}");

      return negative ? source.Where(s => !regex.IsMatch(s)) : source.Where(s => regex.IsMatch(s));
   }
}

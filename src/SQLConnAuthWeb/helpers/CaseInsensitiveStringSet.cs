using System.Collections;

namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Represents a set of strings with case-insensitive uniqueness.
/// </summary>
public class CaseInsensitiveStringSet : IEnumerable<string>
{
   /// <summary>
   /// The underlying set storing the strings, using case-insensitive comparison.
   /// </summary>
   private readonly HashSet<string> _set;

   /// <summary>
   /// Initializes a new, empty instance of <see cref="CaseInsensitiveStringSet"/>.
   /// </summary>
   public CaseInsensitiveStringSet() => _set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

   /// <summary>
   /// Initializes a new instance of <see cref="CaseInsensitiveStringSet"/> containing elements copied from the specified collection.
   /// </summary>
   /// <param name="items">The collection whose elements are copied to the new set.</param>
   public CaseInsensitiveStringSet(IEnumerable<string> items) 
      => _set = new HashSet<string>(items ?? [], StringComparer.OrdinalIgnoreCase);

   /// <summary>
   /// Adds the specified string to the set if it is not already present (case-insensitive).
   /// </summary>
   /// <param name="item">The string to add.</param>
   /// <returns>True if the string was added; false if it was already present.</returns>
   public bool Add(string item) => _set.Add(item);

   /// <summary>
   /// Removes the specified string from the set.
   /// </summary>
   /// <param name="item">The string to remove.</param>
   /// <returns>True if the string was removed; false if it was not found.</returns>
   public bool Remove(string item) => _set.Remove(item);

   /// <summary>
   /// Determines whether the set contains the specified string (case-insensitive).
   /// </summary>
   /// <param name="item">The string to locate in the set.</param>
   /// <returns>True if the set contains the specified string; otherwise, false.</returns>
   public bool Contains(string item) => _set.Contains(item);

   /// <summary>
   /// Gets the number of elements contained in the set.
   /// </summary>
   public int Count => _set.Count;

   /// <summary>
   /// Removes all elements from the set.
   /// </summary>
   public void Clear() => _set.Clear();

   /// <summary>
   /// Returns an enumerator that iterates through the set.
   /// </summary>
   /// <returns>An enumerator for the set.</returns>
   public IEnumerator<string> GetEnumerator() => _set.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   /// <summary>
   /// Copies the elements of the set to a new array.
   /// </summary>
   /// <returns>An array containing copies of the elements of the set.</returns>
   public string[] ToArray() => [.. _set];
}

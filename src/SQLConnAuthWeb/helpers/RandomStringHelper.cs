using System.Security.Cryptography;

namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Provides helper methods for generating random strings.
/// </summary>
public static class RandomStringHelper
{
   /// <summary>
   /// Generates a cryptographically secure random string encoded in a URL-safe Base64 format.
   /// </summary>
   /// <param name="length">The number of random bytes to use for the string. Must be at least 1. Default is 16.</param>
   /// <returns>A URL-safe, Base64-encoded random string.</returns>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length"/> is less than 1.</exception>
   public static string UrlSafeRandomString(int length = 16) {
      ArgumentOutOfRangeException.ThrowIfLessThan(length, 1, nameof(length));

      byte[] randombytes = new byte[length];
      RandomNumberGenerator.Fill(randombytes);

      // URL-safe base64 encoding
      return Convert.ToBase64String(randombytes)
          .Replace("+", "-")
          .Replace("/", "_")
          .TrimEnd('=');
   }
}

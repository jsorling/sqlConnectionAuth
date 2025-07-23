using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents a validated, normalized list of IP addresses or ranges (for allow or deny lists).
/// </summary>
public partial class IPAddressRangeList : IEnumerable<string>
{
   private readonly List<string> _normalizedCidrs = [];

   /// <summary>
   /// Adds an IP address or range (CIDR or subnet mask notation). Throws if invalid.
   /// </summary>
   /// <param name="entry">The IP address or range to add.</param>
   public void Add(string entry) {
      if (string.IsNullOrWhiteSpace(entry))
         throw new ArgumentException("Entry cannot be null or whitespace.", nameof(entry));
      string trimmed = entry.Trim();
      // Check for CIDR notation
      if (CidrNotationRegex().IsMatch(trimmed))
      {
         string[] parts = trimmed.Split('/');
         if (IPAddress.TryParse(parts[0], out IPAddress? ip))
         {
            if (int.TryParse(parts[1], out int prefix))
            {
               if ((ip.AddressFamily == AddressFamily.InterNetwork && prefix >= 0 && prefix <= 32) ||
                   (ip.AddressFamily == AddressFamily.InterNetworkV6 && prefix >= 0 && prefix <= 128))
               {
                  _normalizedCidrs.Add($"{ip}/{prefix}");
                  return;
               }
            }
         }

         throw new FormatException($"Invalid CIDR notation: {trimmed}");
      }
      // Check for subnet mask notation
      if (SubnetMaskNotationRegex().IsMatch(trimmed))
      {
         string[] parts = trimmed.Split('/');
         if (IPAddress.TryParse(parts[0], out IPAddress? ip) && IPAddress.TryParse(parts[1], out IPAddress? mask))
         {
            if (ip.AddressFamily == AddressFamily.InterNetwork && mask.AddressFamily == AddressFamily.InterNetwork)
            {
               int prefix = SubnetMaskToPrefix(mask);
               _normalizedCidrs.Add($"{ip}/{prefix}");
               return;
            }
         }

         throw new FormatException($"Invalid subnet mask notation: {trimmed}");
      }
      // Single IP address
      if (IPAddress.TryParse(trimmed, out IPAddress? singleip))
      {
         int prefix = singleip.AddressFamily == AddressFamily.InterNetwork ? 32 : 128;
         _normalizedCidrs.Add($"{singleip}/{prefix}");
         return;
      }

      throw new FormatException($"Invalid IP address or range: {trimmed}");
   }

   /// <summary>
   /// Removes a normalized CIDR entry.
   /// </summary>
   public bool Remove(string normalizedCidr) => _normalizedCidrs.Remove(normalizedCidr);

   /// <summary>
   /// Gets the count of IP addresses/ranges.
   /// </summary>
   public int Count => _normalizedCidrs.Count;

   /// <summary>
   /// Returns an enumerator for the normalized CIDR entries.
   /// </summary>
   public IEnumerator<string> GetEnumerator() => _normalizedCidrs.GetEnumerator();
   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   /// <summary>
   /// Converts a subnet mask to a prefix length (e.g., 255.255.255.0 -> 24).
   /// </summary>
   private static int SubnetMaskToPrefix(IPAddress mask) {
      byte[] bytes = mask.GetAddressBytes();
      int prefix = 0;
      bool zerofound = false;
      foreach (byte b in bytes)
      {
         for (int i = 7; i >= 0; i--)
         {
            bool bit = (b & (1 << i)) != 0;
            if (zerofound && bit)
            {
               // Non-contiguous mask (e.g., 255.0.255.0)
               throw new FormatException($"Invalid subnet mask: {mask}");
            }

            if (!bit)
               zerofound = true;
            else
               prefix++;
         }
      }

      // Mask must not be all zeros
      return prefix == 0 ? throw new FormatException($"Invalid subnet mask: {mask}") : prefix;
   }

   /// <summary>
   /// Returns a compiled regular expression that matches CIDR notation (e.g., "192.168.1.1/24" or "2001:db8::/64").
   /// </summary>
   /// <returns>A <see cref="Regex"/> instance for matching CIDR notation.</returns>
   [GeneratedRegex(@"^.+/\d{1,3}$")]
   private static partial Regex CidrNotationRegex();

   /// <summary>
   /// Returns a compiled regular expression that matches subnet mask notation (e.g., "192.168.1.1/255.255.255.0").
   /// </summary>
   /// <returns>A <see cref="Regex"/> instance for matching subnet mask notation.</returns>
   [GeneratedRegex(@"^.+/\d+\.\d+\.\d+\.\d+$")]
   private static partial Regex SubnetMaskNotationRegex();
}

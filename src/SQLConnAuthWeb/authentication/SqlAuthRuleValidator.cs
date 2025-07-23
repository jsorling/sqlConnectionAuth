using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.helpers;
using System.Net;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Validates SQL authentication requests against configured security rules and network policies.
/// </summary>
/// <param name="options">The options used to configure rule validation behavior.</param>
public class SqlAuthRuleValidator(IOptions<SqlAuthOptions> options) : ISqlAuthRuleValidator
{
   private readonly SqlAuthOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

   /// <summary>
   /// Validates the provided SQL authentication request against security and network rules.
   /// </summary>
   /// <param name="request">The validation request containing connection and credential information.</param>
   /// <returns>A task that represents the asynchronous operation. The task result contains the rule validation result, including any exception and validated secrets.</returns>
   public async Task<SqlAuthRuleValidationResult> ValidateAsync(SqlAuthValidationRequest request) {
      if (!_options.AllowIntegratedSecurity && request.Password == SqlAuthConsts.WINDOWSAUTHENTICATION)
      {
         return new SqlAuthRuleValidationResult(new ApplicationException("Windows authentication not allowed"), null);
      }

      IPAddress[] ips;
      try
      {
         ips = await IPHelper.ResolveSqlIPAddressAsync(request.Datasource);
      }
      catch
      {
         return new SqlAuthRuleValidationResult(new ApplicationException("Unable to resolve SQL Server address"), null);
      }

      // Filter out invalid/unspecified IPs (e.g., 0.0.0.0 and ::)
      ips = [.. ips.Where(ip => !ip.Equals(IPAddress.Any) && !ip.Equals(IPAddress.IPv6Any))];

      if (ips.Length == 0)
      {
         return new SqlAuthRuleValidationResult(new ApplicationException("No IP address found for SQL Server"), null);
      }

      // If allowed IP addresses list is not null or empty, enforce allow-list logic
      if (_options.AllowedIPAddresses != null && _options.AllowedIPAddresses.Count > 0)
      {
         if (!IsIpAllowed(ips, _options.AllowedIPAddresses))
         {
            return new SqlAuthRuleValidationResult(new ApplicationException("IP address not allowed by allow-list"), null);
         }
      }
      else if (!_options.AllowLoopbackConnections || !_options.AllowPrivateNetworkConnections)
      {
         foreach (IPAddress ip in ips)
         {
            IPNetworkType iptype = IPReservedNetworks.GetIPNetworkType(ip);

            if (iptype == IPNetworkType.Loopback && !_options.AllowLoopbackConnections)
            {
               return new SqlAuthRuleValidationResult(new ApplicationException("Loopback connections not allowed"), null);
            }
            else if (iptype != IPNetworkType.Public && !_options.AllowPrivateNetworkConnections)
            {
               return new SqlAuthRuleValidationResult(new ApplicationException("Private network connections not allowed"), null);
            }
         }
      }

      return new SqlAuthRuleValidationResult(null, new(
          Password: request.Password
          , TrustServerCertificate: request.TrustServerCertificate
          , RuleReValidationAfter: DateTime.UtcNow.AddMinutes(5)
      ));
   }

   /// <summary>
   /// Checks if any of the given IP addresses are allowed by the allow-list.
   /// </summary>
   private static bool IsIpAllowed(IPAddress[] ips, IPAddressRangeList allowedList)
   {
      foreach (IPAddress ip in ips)
      {
         foreach (string cidr in allowedList)
         {
            if (IsIpInCidr(ip, cidr))
               return true;
         }
      }

      return false;
   }

   /// <summary>
   /// Checks if an IP address is within a CIDR range.
   /// </summary>
   private static bool IsIpInCidr(IPAddress ip, string cidr)
   {
      string[] parts = cidr.Split('/');
      if (parts.Length != 2) return false;
      if (!IPAddress.TryParse(parts[0], out IPAddress? networkip)) return false;
      if (!int.TryParse(parts[1], out int prefixlength)) return false;

      byte[] ipbytes = ip.GetAddressBytes();
      byte[] networkbytes = networkip.GetAddressBytes();
      if (ipbytes.Length != networkbytes.Length) return false;

      int bits = prefixlength;
      for (int i = 0; i < ipbytes.Length; i++)
      {
         int mask = bits >= 8 ? 255 : bits > 0 ? (byte)~(255 >> bits) : 0;
         if ((ipbytes[i] & mask) != (networkbytes[i] & mask))
            return false;
         bits -= 8;
         if (bits <= 0) break;
      }

      return true;
   }
}


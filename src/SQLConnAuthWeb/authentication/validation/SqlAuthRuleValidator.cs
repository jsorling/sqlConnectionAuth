using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.helpers;
using System.Net;
using System.Net.Sockets;

namespace Sorling.SqlConnAuthWeb.authentication.validation;

/// <summary>
/// Validates SQL authentication requests against configured security rules and network policies.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqlAuthRuleValidator"/> class.
/// </remarks>
/// <param name="optionsMonitor">The options monitor used to retrieve current rule validation options at runtime.</param>
public class SqlAuthRuleValidator(IOptionsMonitor<SqlAuthOptions> optionsMonitor, ISqlAuthDatabaseNameFilter databaseNameValidator) : ISqlAuthRuleValidator
{
   private readonly IOptionsMonitor<SqlAuthOptions> _optionsMonitor
      = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));

   private readonly ISqlAuthDatabaseNameFilter _databaseNameValidator
      = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));

   /// <inheritdoc/>
   public async Task<SqlAuthRuleValidationResult> ValidateConnectionAsync(SqlAuthValidationRequest request, string? dbName) {
      SqlAuthOptions options = _optionsMonitor.CurrentValue;
      if (!options.AllowIntegratedSecurity && request.Password == SqlAuthConsts.WINDOWSAUTHENTICATION)
         return new SqlAuthRuleValidationResult(new ApplicationException("Windows authentication not allowed"), null);

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
         return new SqlAuthRuleValidationResult(new ApplicationException("No IP address found for SQL Server"), null);

      // If allowed IP addresses list is not null or empty, enforce allow-list logic
      if (options.AllowedIPAddresses != null && options.AllowedIPAddresses.Count > 0)
      {
         if (!IsIpAllowed(ips, options.AllowedIPAddresses))
         {
            return new SqlAuthRuleValidationResult(
               new ApplicationException(
                  $"IP address not allowed by allow-list ({string.Join(',', ips.Select(s => s.ToString()))})"), null);
         }
      }
      else if (!options.AllowLoopbackConnections || !options.AllowPrivateNetworkConnections)
      {
         foreach (IPAddress ip in ips)
         {
            IPNetworkType iptype = IPReservedNetworks.GetIPNetworkType(ip);

            if (iptype == IPNetworkType.Loopback && !options.AllowLoopbackConnections)
               return new SqlAuthRuleValidationResult(new ApplicationException("Loopback connections not allowed"), null);
            else if (iptype != IPNetworkType.Public && !options.AllowPrivateNetworkConnections)
               return new SqlAuthRuleValidationResult(new ApplicationException("Private network connections not allowed"), null);
         }
      }

      return new SqlAuthRuleValidationResult(null, new(
          Password: request.Password
          , TrustServerCertificate: request.TrustServerCertificate
          , RuleReValidationAfter: DateTime.UtcNow.AddMinutes(5)
          , DBName: dbName
      ));
   }

   /// <summary>
   /// Checks if any of the given IP addresses are allowed by the allow-list.
   /// </summary>
   /// <param name="ips">The IP addresses to check.</param>
   /// <param name="allowedList">The allow-list of IP address ranges.</param>
   /// <returns>True if any IP is allowed; otherwise, false.</returns>
   private static bool IsIpAllowed(IPAddress[] ips, IPAddressRangeList allowedList) {
      foreach (IPAddress ip in ips)
         foreach (string cidr in allowedList)
            if (IsIpInCidr(ip, cidr))
               return true;

      return false;
   }

   /// <summary>
   /// Checks if an IP address is within a CIDR range.
   /// </summary>
   /// <param name="ip">The IP address to check.</param>
   /// <param name="cidr">The CIDR range string.</param>
   /// <returns>True if the IP is within the range; otherwise, false.</returns>
   private static bool IsIpInCidr(IPAddress ip, string cidr) {
      if (string.IsNullOrWhiteSpace(cidr))
         return false;

      string[] parts = cidr.Trim().Split('/');
      if (parts.Length != 2)
         return false;
      if (!IPAddress.TryParse(parts[0], out IPAddress? networkip))
         return false;
      if (!int.TryParse(parts[1], out int prefixlength))
         return false;

      // Validate prefix length according to address family
      if (networkip.AddressFamily == AddressFamily.InterNetwork)
      {
         if (prefixlength is < 0 or > 32)
            return false;
      }
      else if (networkip.AddressFamily == AddressFamily.InterNetworkV6)
      {
         if (prefixlength is < 0 or > 128)
            return false;
      }
      else
      {
         return false;
      }

      // Normalize address families (handle IPv4-mapped IPv6)
      if (networkip.AddressFamily == AddressFamily.InterNetwork && ip.AddressFamily == AddressFamily.InterNetworkV6 && ip.IsIPv4MappedToIPv6)
      {
         ip = ip.MapToIPv4();
      }

      byte[] ipbytes = ip.GetAddressBytes();
      byte[] networkbytes = networkip.GetAddressBytes();

      if (ipbytes.Length != networkbytes.Length)
         return false;

      static byte MaskByte(int remainingBits) => remainingBits >= 8
         ? (byte)0xFF
         : remainingBits <= 0
            ? (byte)0x00
            : (byte)(0xFF << (8 - remainingBits));

      int bits = prefixlength;
      for (int i = 0; i < ipbytes.Length && bits > 0; i++)
      {
         byte mask = MaskByte(bits);
         if ((ipbytes[i] & mask) != (networkbytes[i] & mask))
            return false;
         bits -= 8;
      }

      return true;
   }

   public Task<bool> ValidateDatabaseAsync(string databaseName)
      => Task.FromResult(_databaseNameValidator.IsAllowed(databaseName));
}


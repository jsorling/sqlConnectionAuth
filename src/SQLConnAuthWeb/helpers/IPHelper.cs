using System.Net;

namespace Sorling.SqlConnAuthWeb.helpers;

public static class IPHelper
{
   public static string GetSQLServerAddress(string dataSource) {
      ArgumentException.ThrowIfNullOrEmpty(dataSource, nameof(dataSource));

      string tor = dataSource;

      if (tor.Contains('\\'))
      {
         tor = tor.Split('\\')[0];
      }

      if (tor.Contains(','))
      {
         tor = tor.Split(',')[0];
      }

      return tor.Trim();
   }

   public static string GetSQLServerInstance(string dataSource) {
      ArgumentException.ThrowIfNullOrEmpty(dataSource, nameof(dataSource));

      string tor = dataSource;

      if (tor.Contains('\\'))
      {
         tor = tor.Split(',')[0];
         tor = tor.Split('\\')[1];
      }
      else if (tor.Contains(','))
      {
         tor = tor.Split(',')[0];
      }
      else
      {
         return string.Empty;
      }

      return tor.Trim();
   }

   public static int GetSQLServerPort(string dataSource) {
      ArgumentException.ThrowIfNullOrEmpty(dataSource, nameof(dataSource));

      return dataSource.Contains(',') && int.TryParse(dataSource.Split(',')[^1].Trim(), out int result) ? result : 1433;
   }

   public static async Task<IPAddress[]> ResolveSqlIPAddressAsync(string dataSource) {
      string host = GetSQLServerAddress(dataSource);

      return IPAddress.TryParse(host, out IPAddress? ipaddress)
         ? [ipaddress]
         : host.Trim() == "."
         ? [IPAddress.Loopback, IPAddress.IPv6Loopback]
         : await Dns.GetHostAddressesAsync(host);
   }
}


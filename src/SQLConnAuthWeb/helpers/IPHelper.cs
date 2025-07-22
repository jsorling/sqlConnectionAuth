using System.Net;

namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Provides helper methods for parsing and resolving SQL Server data source addresses, instances, and ports.
/// </summary>
public static class IPHelper
{
   /// <summary>
   /// Extracts the SQL Server address (host) from a data source string.
   /// </summary>
   /// <param name="dataSource">The SQL Server data source string.</param>
   /// <returns>The server address or host name.</returns>
   /// <exception cref="ArgumentException">Thrown if <paramref name="dataSource"/> is null or empty.</exception>
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

   /// <summary>
   /// Extracts the SQL Server instance name from a data source string, if present.
   /// </summary>
   /// <param name="dataSource">The SQL Server data source string.</param>
   /// <returns>The instance name, or an empty string if not present.</returns>
   /// <exception cref="ArgumentException">Thrown if <paramref name="dataSource"/> is null or empty.</exception>
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

   /// <summary>
   /// Extracts the SQL Server port from a data source string, or returns the default port (1433) if not specified.
   /// </summary>
   /// <param name="dataSource">The SQL Server data source string.</param>
   /// <returns>The port number.</returns>
   /// <exception cref="ArgumentException">Thrown if <paramref name="dataSource"/> is null or empty.</exception>
   public static int GetSQLServerPort(string dataSource) {
      ArgumentException.ThrowIfNullOrEmpty(dataSource, nameof(dataSource));

      return dataSource.Contains(',') && int.TryParse(dataSource.Split(',')[^1].Trim(), out int result) ? result : 1433;
   }

   /// <summary>
   /// Resolves the IP addresses for the given SQL Server data source string.
   /// </summary>
   /// <param name="dataSource">The SQL Server data source string.</param>
   /// <returns>An array of resolved <see cref="IPAddress"/> objects.</returns>
   public static async Task<IPAddress[]> ResolveSqlIPAddressAsync(string dataSource) {
      string host = GetSQLServerAddress(dataSource);

      return IPAddress.TryParse(host, out IPAddress? ipaddress)
          ? [ipaddress]
          : host.Trim() == "."
          ? [IPAddress.Loopback, IPAddress.IPv6Loopback]
          : await Dns.GetHostAddressesAsync(host);
   }
}


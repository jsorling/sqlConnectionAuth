using System.Net;
using IPNetwork = System.Net.IPNetwork;

namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Provides reserved IP network definitions and logic to determine the network type of an IP address.
/// </summary>
public static class IPReservedNetworks
{
   /// <summary>
   /// Reserved private IP networks (IPv4 and IPv6).
   /// </summary>
   public static readonly IPNetwork[] PRIVATE_NETWORKS = [
       new (IPAddress.Parse("10.0.0.0"), 8)
        , new (IPAddress.Parse("100.64.0.0"), 10)
        , new (IPAddress.Parse("172.16.0.0"), 12)
        , new (IPAddress.Parse("192.0.0.0"), 24)
        , new (IPAddress.Parse("192.168.0.0"), 16)
        , new (IPAddress.Parse("198.18.0.0"), 15)
        , new (IPAddress.Parse("64:ff9b:1::"), 48)
        , new (IPAddress.Parse("100::"), 64)
        , new (IPAddress.Parse("5f00::"), 16)
        , new (IPAddress.Parse("fc00::"), 7)
        , new (IPAddress.Parse("fd00::"), 8)
        , new (IPAddress.Parse("169.254.0.0"), 16)
        , new (IPAddress.Parse("255.255.255.255"), 32)
        , new (IPAddress.Parse("100::"), 64)
        , new (IPAddress.Parse("5f00::"), 16)
        , new (IPAddress.Parse("fe80::"), 10)
   ];

   /// <summary>
   /// Reserved loopback IP networks (IPv4 and IPv6).
   /// </summary>
   public static readonly IPNetwork[] LOOPBACK_NETWORKS = [
       new (IPAddress.Parse("127.0.0.0"), 8)
        , new (IPAddress.Parse("::1"), 128)
   ];

   /// <summary>
   /// Other reserved IP networks (IPv4 and IPv6) not classified as private or loopback.
   /// </summary>
   public static readonly IPNetwork[] OTHER_NETWORKS = [
       new (IPAddress.Parse("0.0.0.0"), 8)
        , new (IPAddress.Parse("192.0.2.0"), 24)
        , new (IPAddress.Parse("198.51.100.0"), 24)
        , new (IPAddress.Parse("203.0.113.0"), 24)
        , new (IPAddress.Parse("233.252.0.0"), 24)
        , new (IPAddress.Parse("::"), 128)
        , new (IPAddress.Parse("::ffff:0:0"), 96)
        , new (IPAddress.Parse("::ffff:0:0:0"), 96)
        , new (IPAddress.Parse("2001:20::"), 28)
        , new (IPAddress.Parse("2001:db8::"), 32)
        , new (IPAddress.Parse("3fff::"), 20)
   ];

   /// <summary>
   /// Determines the <see cref="IPNetworkType"/> for the specified <see cref="IPAddress"/>.
   /// </summary>
   /// <param name="ipAddress">The IP address to classify.</param>
   /// <returns>The network type of the IP address.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="ipAddress"/> is null.</exception>
   public static IPNetworkType GetIPNetworkType(IPAddress ipAddress) {
      ArgumentNullException.ThrowIfNull(ipAddress, nameof(ipAddress));

      foreach (IPNetwork network in PRIVATE_NETWORKS)
      {
         if (network.Contains(ipAddress))
            return IPNetworkType.Private;
      }

      foreach (IPNetwork network in LOOPBACK_NETWORKS)
      {
         if (network.Contains(ipAddress))
            return IPNetworkType.Loopback;
      }

      foreach (IPNetwork network in OTHER_NETWORKS)
      {
         if (network.Contains(ipAddress))
            return IPNetworkType.Other;
      }

      return IPNetworkType.Public;
   }
}

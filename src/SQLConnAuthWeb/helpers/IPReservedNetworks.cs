using System.Net;
using IPNetwork = System.Net.IPNetwork;

namespace Sorling.SqlConnAuthWeb.helpers;

//https://en.wikipedia.org/wiki/Reserved_IP_addresses
//https://www.iana.org/assignments/iana-ipv4-special-registry/iana-ipv4-special-registry.xhtml
public static class IPReservedNetworks
{
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

   public static readonly IPNetwork[] LOOPBACK_NETWORKS = [
      new (IPAddress.Parse("127.0.0.0"), 8)
      , new (IPAddress.Parse("::1"), 128)
   ];

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

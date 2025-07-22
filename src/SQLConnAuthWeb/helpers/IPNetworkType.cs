namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Specifies the type of IP network for a given IP address.
/// </summary>
public enum IPNetworkType
{
   /// <summary>
   /// The IP address is public (not reserved for private or loopback use).
   /// </summary>
   Public,
   /// <summary>
   /// The IP address is private (reserved for internal network use).
   /// </summary>
   Private,
   /// <summary>
   /// The IP address is a loopback address (e.g., 127.0.0.1).
   /// </summary>
   Loopback,
   /// <summary>
   /// The IP address does not fit into the other categories.
   /// </summary>
   Other
}

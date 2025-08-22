using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Provides configuration options for SQL authentication and connection settings.
/// </summary>
public partial class SqlAuthOptions
{
   /// <summary>
   /// Gets or sets a value indicating whether integrated security (Windows Authentication) is allowed for SQL connections.
   /// </summary>
   public bool AllowIntegratedSecurity { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether the option to trust the SQL Server certificate is allowed.
   /// </summary>
   public bool AllowTrustServerCertificate { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether connections to loopback addresses (e.g., localhost, 127.0.0.1) are permitted.
   /// </summary>
   public bool AllowLoopbackConnections { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether connections to private network addresses are permitted.
   /// </summary>
   public bool AllowPrivateNetworkConnections { get; set; }

   /// <summary>
   /// Gets or sets the list of allowed IP addresses or ranges (CIDR or subnet mask notation).
   /// </summary>
   public IPAddressRangeList AllowedIPAddresses { get; set; } = [];

   /// <summary>
   /// Gets or sets the list of database names to include in the filter. Case-insensitive, no duplicates.
   /// </summary>
   public CaseInsensitiveStringSet IncludeDatabaseFilter { get; set; } = [];

   /// <summary>
   /// Gets or sets the list of database names to exclude from the filter. Case-insensitive, no duplicates.
   /// </summary>
   public CaseInsensitiveStringSet ExcludeDatabaseFilter { get; set; } = [];
}

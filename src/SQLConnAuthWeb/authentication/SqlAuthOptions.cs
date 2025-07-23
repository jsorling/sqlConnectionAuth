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
   /// Gets or sets the name of the local storage key used for the theme switcher. If set, enables the use of a theme switcher in the UI.
   /// </summary>
   public string? ThemeSwitcherLocalStorageName { get; set; }

   /// <summary>
   /// Determines whether the theme switcher should be used based on the presence of a local storage key name.
   /// </summary>
   /// <returns>True if <see cref="ThemeSwitcherLocalStorageName"/> is not null or empty; otherwise, false.</returns>
   public bool UseThemeSwitcher() => !string.IsNullOrEmpty(ThemeSwitcherLocalStorageName);

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
   public IPAddressRangeList AllowedIPAddresses { get; set; } = new IPAddressRangeList();
}

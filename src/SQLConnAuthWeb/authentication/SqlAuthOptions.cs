namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlAuthOptions
{
   public bool AllowIntegratedSecurity { get; set; }

   public string? ThemeSwitcherLocalStorageName { get; set; }

   public bool UseThemeSwitcher() => !string.IsNullOrEmpty(ThemeSwitcherLocalStorageName);

   public bool AllowTrustServerCertificate { get; set; }

   public bool AllowLoopbackConnections { get; set; }

   public bool AllowPrivateNetworkConnections { get; set; }   
}

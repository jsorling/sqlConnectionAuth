namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlConnAuthenticationOptions
{
   public string SqlRootPath { get; set; } = "/db";

   public bool AllowWinauth { get; set; } = false;

   public string? ThemeSwitcherLocalStorageName {  get; set; } = null;

   public bool UseThemeSwitcher() => !string.IsNullOrEmpty(ThemeSwitcherLocalStorageName);

   public bool AllowTrustServerCertificate { get; set; } = false;
}

namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlConnAuthenticationOptions
{
   public string SqlPath { get; set; } = "/db";

   public bool AllowWinauth { get; set; } = false;

   public string? ThemeSwitcherLocalStorageName {  get; set; } = null;

   public bool UseThemeSwitcher() => !string.IsNullOrEmpty(ThemeSwitcherLocalStorageName);
}

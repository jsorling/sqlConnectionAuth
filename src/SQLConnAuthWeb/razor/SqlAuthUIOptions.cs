namespace Sorling.SqlConnAuthWeb.razor;

public class SqlAuthUIOptions
{
   /// <summary>
   /// Gets or sets the name of the local storage key used for the theme switcher. If set, enables the use of a theme switcher in the UI.
   /// </summary>
   public string? ThemeSwitcherLocalStorageName { get; set; }

   /// <summary>
   /// Gets or sets the background color for the light theme.
   /// </summary>
   public string LightBackgroundColor { get; set; } = "#fff";

   /// <summary>
   /// Gets or sets the foreground color for the light theme.
   /// </summary>
   public string LightForegroundColor { get; set; } = "#222";

   /// <summary>
   /// Gets or sets the background color for the dark theme.
   /// </summary>
   public string DarkBackgroundColor { get; set; } = "#181818";

   /// <summary>
   /// Gets or sets the foreground color for the dark theme.
   /// </summary>
   public string DarkForegroundColor { get; set; } = "#f5f5f5";

   /// <summary>
   /// Determines whether the theme switcher should be used based on the presence of a local storage key name.
   /// </summary>
   /// <returns>True if <see cref="ThemeSwitcherLocalStorageName"/> is not null or empty; otherwise, false.</returns>
   public bool UseThemeSwitcher() => !string.IsNullOrEmpty(ThemeSwitcherLocalStorageName);
}

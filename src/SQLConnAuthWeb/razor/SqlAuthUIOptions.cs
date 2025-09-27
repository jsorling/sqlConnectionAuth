namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// UI configuration options for the SQL Connection Authentication Razor Pages UI, such as theme colors,
/// theme switcher behavior, and recent-items behavior used by client-side scripts and tag helpers.
/// </summary>
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

   /// <summary>
   /// Gets or sets the maximum number of recent items to keep in local storage.
   /// This value determines the maximum number of server/user pairs (first dimension)
   /// and the maximum number of databases per server/user (second dimension) to track.
   /// When the limit is exceeded, the least recently used items are removed. Used for
   /// managing the recent items array in local storage, which is updated and truncated
   /// as needed when submitting forms such as <c>index.cshtml</c> and <c>selectdb.cshtml</c>.
   /// </summary>
   public int MaxRecentItems { get; set; }

   /// <summary>
   /// Determines whether the recent items feature is enabled.
   /// Returns <c>true</c> if <see cref="MaxRecentItems"/> is greater than zero, allowing
   /// the UI to track and display recent server/user/database selections in a two-dimensional array.
   /// The first dimension tracks server/user pairs, and the second dimension tracks databases for each pair.
   /// </summary>
   /// <returns>
   /// <c>true</c> if recent items should be tracked; otherwise, <c>false</c>.
   /// </returns>
   public bool UseRecentItems() => MaxRecentItems > 0;
}

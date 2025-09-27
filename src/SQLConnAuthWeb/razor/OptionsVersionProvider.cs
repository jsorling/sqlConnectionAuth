using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// Provides a global version string for cache busting, based on a timestamp.
/// Updates the version whenever SqlAuthUIOptions or SqlAuthOptions changes at runtime.
/// </summary>
public class OptionsVersionProvider
{
    private long _versionTicks;

    /// <summary>
    /// Gets a version string derived from the latest timestamp when options changed.
    /// Can be used for cache-busting URLs.
    /// </summary>
    public string Version => _versionTicks.ToString();

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsVersionProvider"/> class and wires up change listeners
    /// for <see cref="SqlAuthUIOptions"/> and <see cref="SqlAuthOptions"/> to update the version when options change.
    /// </summary>
    /// <param name="ui">The options monitor for UI options.</param>
    /// <param name="auth">The options monitor for authentication options.</param>
    public OptionsVersionProvider(IOptionsMonitor<SqlAuthUIOptions> ui, IOptionsMonitor<SqlAuthOptions> auth)
    {
        _versionTicks = DateTime.UtcNow.Ticks;
      _ = ui.OnChange(_ => _versionTicks = DateTime.UtcNow.Ticks);
      _ = auth.OnChange(_ => _versionTicks = DateTime.UtcNow.Ticks);
    }
}

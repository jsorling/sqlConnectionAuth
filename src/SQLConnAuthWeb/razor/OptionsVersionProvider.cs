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
    public string Version => _versionTicks.ToString();

    public OptionsVersionProvider(IOptionsMonitor<SqlAuthUIOptions> ui, IOptionsMonitor<SqlAuthOptions> auth)
    {
        _versionTicks = DateTime.UtcNow.Ticks;
      _ = ui.OnChange(_ => _versionTicks = DateTime.UtcNow.Ticks);
      _ = auth.OnChange(_ => _versionTicks = DateTime.UtcNow.Ticks);
    }
}

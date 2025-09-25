using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

public class ThemeSwitcherModel(IOptionsMonitor<SqlAuthUIOptions> uiOptions
   , IOptionsMonitor<SqlAuthOptions> authOptions
   , OptionsVersionProvider versionProvider) : PageModel
{
   /// <summary>
   /// Gets the SQL UI options.
   /// </summary>
   public SqlAuthUIOptions SqlAuthUIOptions { get; } = uiOptions.CurrentValue;

   /// <summary>
   /// Gets the SQL Auth options.
   /// </summary>
   public SqlAuthOptions SqlAuthOptions { get; } = authOptions.CurrentValue;

   /// <summary>
   /// Gets a version string that changes if options change (simple timestamp).
   /// </summary>
   public string Version => versionProvider.Version;

   /// <summary>
   /// Handles GET requests to the ThemeSwitcherJS page.
   /// Sets cache headers for browser caching.
   /// </summary>
   /// <returns>The page result.</returns>
   public IActionResult OnGet() {
      Response.Headers.CacheControl = "public, max-age=31536000, immutable";
      Response.ContentType = "text/javascript; charset=utf-8";
      return Page();
   }
}

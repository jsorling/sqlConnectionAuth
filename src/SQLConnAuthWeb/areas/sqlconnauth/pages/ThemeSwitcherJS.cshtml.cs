using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.razor;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for serving the theme switcher JavaScript, using SQL UI options.
/// </summary>
public class ThemeSwitcherJS(IOptionsMonitor<SqlAuthUIOptions> options) : PageModel
{
   /// <summary>
   /// Gets the SQL UI options.
   /// </summary>
   public SqlAuthUIOptions SqlAuthUIOptions => options.CurrentValue;

   /// <summary>
   /// Handles GET requests to the ThemeSwitcherJS page.
   /// </summary>
   /// <returns>The page result.</returns>
   public IActionResult OnGet() => Page();
}

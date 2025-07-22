using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for serving the theme switcher JavaScript, using SQL authentication options.
/// </summary>
/// <param name="options">The SQL authentication options.</param>
public class ThemeSwitcherJS(IOptions<SqlAuthOptions> options) : PageModel
{
   /// <summary>
   /// Gets the SQL authentication options.
   /// </summary>
   public SqlAuthOptions SQLAuthOptions { get; } = options.Value ?? throw new ArgumentNullException(nameof(options));

   /// <summary>
   /// Handles GET requests to the ThemeSwitcherJS page.
   /// </summary>
   /// <returns>The page result.</returns>
   public IActionResult OnGet() => Page();
}

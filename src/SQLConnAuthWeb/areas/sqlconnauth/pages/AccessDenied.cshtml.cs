using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for the Access Denied page in the SQL authentication area.
/// </summary>
[AllowAnonymous]
[RequireHttps]
public class AccessDeniedModel : PageModel
{
    /// <summary>
    /// Handles GET requests to the Access Denied page.
    /// </summary>
    /// <returns>The page result.</returns>
    public IActionResult OnGet() => Page();
}

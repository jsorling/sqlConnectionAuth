using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

public class ThemeSwitcherJS(ISqlConnAuthenticationService sqlConnAuthenticationService) : PageModel
{
   public ISqlConnAuthenticationService SqlConnAuthentication { get; init; } = sqlConnAuthenticationService;

   public IActionResult OnGet() => Page();
}

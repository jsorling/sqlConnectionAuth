using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

public class ThemeSwitcherJS : PageModel
{
   public ISqlConnAuthenticationService SqlConnAuthentication { get; init; }

   public ThemeSwitcherJS(ISqlConnAuthenticationService sqlConnAuthenticationService)=> SqlConnAuthentication = sqlConnAuthenticationService;

   public IActionResult OnGet() => Page();
}

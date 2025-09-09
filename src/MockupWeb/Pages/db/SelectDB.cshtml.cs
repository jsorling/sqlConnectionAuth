using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace MockupWeb.Pages.db;

public class SelectDBModel(ISqlAuthContext context) : PageModel
{
   public IActionResult OnGet() => context.GetRedirectToSelectDBActionResult(null);
}

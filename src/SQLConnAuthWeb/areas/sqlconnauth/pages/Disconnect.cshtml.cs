using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

public class DisconnectModel(ISqlAuthService sqlConAuthService) : PageModel
{
   private readonly ISqlAuthService _sqlconauth = sqlConAuthService;

   public async Task<IActionResult> OnGetAsync(string? returnUrl = null) {
      await _sqlconauth.SignoutAsync();

      return returnUrl is not null ? Redirect(returnUrl) : Page();
   }
}

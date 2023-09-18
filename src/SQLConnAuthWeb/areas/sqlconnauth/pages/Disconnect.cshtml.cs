using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

public class DisconnectModel : PageModel
{
   private readonly ISqlConnAuthenticationService _sqlConnAuthentication;

   public DisconnectModel(ISqlConnAuthenticationService sqlConnAuthenticationService)
      => _sqlConnAuthentication = sqlConnAuthenticationService;

   public async Task<IActionResult> OnGetAsync(string? returnUrl = null) {
      await _sqlConnAuthentication.SignoutAsync();

      return returnUrl is not null ? Redirect(returnUrl) : Page();
   }
}

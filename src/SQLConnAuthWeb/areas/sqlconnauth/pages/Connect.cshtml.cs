using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class ConnectModel(ISqlConnAuthenticationService sqlConnAuthenticationService) : PageModel
{
   [BindProperty]
   public InputAuthenticationModel Input { get; set; } = new();

   private readonly ISqlConnAuthenticationService _sqlConnAuthentication = sqlConnAuthenticationService;

   public bool IsWinAuth { get; private set; }

   public void OnGet([FromRoute] string sqlauthparamsrv, [FromRoute] string sqlauthparamusr) {
      IsWinAuth = sqlauthparamusr == SqlConnAuthConsts.WINDOWSAUTHENTICATION && _sqlConnAuthentication.Options.AllowWinauth;
      Input = new() { SqlServer = sqlauthparamsrv, UserName = sqlauthparamusr };
   }

   public async Task<IActionResult> OnPostAsync([FromRoute] string sqlauthparamsrv, [FromRoute] string sqlauthparamusr
      , string? returnUrl = null) {
      if (ModelState.IsValid) {
         IsWinAuth = sqlauthparamusr == SqlConnAuthConsts.WINDOWSAUTHENTICATION && _sqlConnAuthentication.Options.AllowWinauth;
         SqlConnAuthenticationResult result = await _sqlConnAuthentication.AuthenticateAsync(sqlauthparamsrv
            , sqlauthparamusr, new(Input.Password, Input.TrustServerCertificate));

         if (!result.Success && result.Exception is not null) {
            ModelState.AddModelError("Password", result.Exception.Message);
         }

         return result.Success ? Redirect(returnUrl ?? _sqlConnAuthentication.UriEscapedPath(sqlauthparamsrv, sqlauthparamusr))
            : Page();
      }

      return Page();
   }
}

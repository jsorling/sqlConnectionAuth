using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class ConnectModel : PageModel
{
   [BindProperty]
   public SqlConnAuthenticationModel? Input { get; set; }

   private readonly ISqlConnAuthenticationService _sqlConnAuthentication;

   public bool IsWinAuth { get; private set; }

   public ConnectModel(ISqlConnAuthenticationService sqlConnAuthenticationService)
      => _sqlConnAuthentication = sqlConnAuthenticationService;

   public void OnGet([FromRoute] string sqlauthparamsrv, [FromRoute] string sqlauthparamusr) {
      IsWinAuth = sqlauthparamusr == SqlConnAuthConsts.WINDOWSAUTHENTICATION && _sqlConnAuthentication.Options.AllowWinauth;
      Input = new() { SqlServer = sqlauthparamsrv, UserName = sqlauthparamusr };
   }

   public async Task<IActionResult> OnPostAsync([FromRoute] string sqlauthparamsrv, [FromRoute] string sqlauthparamusr, string? returnUrl = null) {
      if (ModelState.IsValid) {
         IsWinAuth = sqlauthparamusr == SqlConnAuthConsts.WINDOWSAUTHENTICATION && _sqlConnAuthentication.Options.AllowWinauth;
         SqlConnAuthenticationResult result = await _sqlConnAuthentication.AuthenticateAsync(sqlauthparamsrv
            , sqlauthparamusr, Input!.Password!);

         if (!result.Success && result.Exception is not null) {
            ModelState.AddModelError("Password", result.Exception.Message);
         }

         return result.Success ? Redirect(returnUrl ?? _sqlConnAuthentication.UriEscapedPath(sqlauthparamsrv, sqlauthparamusr)) : Page();
      }

      return Page();
   }
}

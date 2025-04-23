using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class ConnectModel(ISqlAuthService sqlConnAuthenticationService) : PageModel
{
   [BindProperty]
   public InputPasswordModel Input { get; set; } = new();

   private readonly ISqlAuthService _sqlConnAuthentication = sqlConnAuthenticationService;

   public bool IsWinAuth { get; private set; }

   public string SQLServer => _sqlConnAuthentication.SQLServer;

   public string UserName => _sqlConnAuthentication.UserName;

   public void OnGet([FromRoute] string sqlauthparamusr) => IsWinAuth = sqlauthparamusr == SqlAuthConsts.WINDOWSAUTHENTICATION && _sqlConnAuthentication.Options.AllowIntegratedSecurity;

   public async Task<IActionResult> OnPostAsync([FromRoute] string sqlauthparamusr
      , string? returnUrl = null) {
      if (ModelState.IsValid)
      {
         IsWinAuth = sqlauthparamusr == SqlAuthConsts.WINDOWSAUTHENTICATION && _sqlConnAuthentication.Options.AllowIntegratedSecurity;
         SqlAuthenticationResult result = await _sqlConnAuthentication.AuthenticateAsync(Input);

         if (!result.Success && result.Exception is not null)
         {
            ModelState.AddModelError("Password", result.Exception.Message);
         }

         return result.Success ? Redirect(returnUrl ?? _sqlConnAuthentication.UriEscapedPath)
            : Page();
      }

      return Page();
   }
}

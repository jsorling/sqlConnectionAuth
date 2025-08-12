using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class SelectDBModel(ISqlAuthService sqlConnAuthenticationService) : PageModel
{
   private readonly ISqlAuthService _sqlConnAuthentication = sqlConnAuthenticationService;

   [BindProperty]
   public InputSelectDBModel Input { get; set; } = new();

   public async void OnGetAsync([FromRoute] string sqlauthparamtemppwd) {
      SqlAuthenticationResult tresult = await _sqlConnAuthentication.TestAuthenticateAsync(sqlauthparamtemppwd, null);
      if (tresult is not null && !tresult.Success) {
         ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
         return;
      }

      if (tresult.Success) {
         //Input.Databases = await _sqlConnAuthentication.GetDBsAsync();
         //Input.SqlServer = _sqlConnAuthentication.SQLServer;
         //Input.UserName = _sqlConnAuthentication.UserName;
         //Input.UriEscapedPath = _sqlConnAuthentication.UriEscapedPath;
         //Input.SqlVersion = tresult.SqlVersion;
      } else {
         ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
      }
   }
}

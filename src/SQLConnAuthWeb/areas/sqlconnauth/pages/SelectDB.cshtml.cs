using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.exceptions;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class SelectDBModel(ISqlAuthService sqlConnAuthenticationService) : PageModel
{
   private readonly ISqlAuthService _sqlConnAuthentication = sqlConnAuthenticationService;

   [BindProperty]
   public InputSelectDBModel Input { get; set; } = new();

   public async Task<IActionResult> OnGetAsync([FromRoute] string sqlauthparamtemppwd) {
      SqlAuthenticationResult tresult = await _sqlConnAuthentication.TestAuthenticateAsync(sqlauthparamtemppwd, null)
         ?? throw new NullReferenceException($"Error in the implementation of {nameof(ISqlAuthService)} returns null");

      if (tresult.Exception is TemporaryPasswordNotFoundException)
      {
         // redirect to reenter the temporary password
         RouteValueDictionary routevalues = new()
         {
            { "area", SqlAuthConsts.SQLAUTHAREA },
            { SqlAuthConsts.URLROUTEPARAMSRV, HttpContext.GetSqlAuthServer() },
            { SqlAuthConsts.URLROUTEPARAMUSR, HttpContext.GetSqlAuthUserName() }
         };

         return RedirectToPage("connect", routevalues);
      }
      else if (!tresult.Success)
      {
         ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
         return Page();
      }
      else
      {
         //Input.Databases = await _sqlConnAuthentication.GetDBsAsync();
         return Page();
      }
   }
}

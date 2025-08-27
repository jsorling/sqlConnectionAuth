using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class SelectDBModel(ISqlAuthService sqlConnAuthenticationService
   , ISqlAuthDBAccess sqlAuthDBAccess
   , ISqlAuthPwdStore pwdStore) : PageModel
{
   [BindProperty]
   public InputSelectDBModel Input { get; set; } = new();

   private async Task<SqlAuthenticationResult> ValidateTmpPwdAsync(SqlAuthTempPasswordInfo tmpPwd)
      => await sqlConnAuthenticationService.TestAuthenticateAsync(tmpPwd, null);
   //?? throw new NullReferenceException($"Error in the implementation of {nameof(ISqlAuthService)} returns null");

   private async Task<SqlAuthTempPasswordInfo?> GetTempPasswordInfoAsync() {
      string? tmppwdkey = Request.RouteValues[SqlAuthConsts.URLROUTEPARAMTEMPPWD] as string;
      return string.IsNullOrEmpty(tmppwdkey) ? null : await pwdStore.PeekTempPasswordAsync(tmppwdkey);
   }

   private async Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthTempPasswordInfo tmpPwd)
      => await sqlAuthDBAccess.GetDatabasesAsync(new(HttpContext.GetSqlAuthServer(), HttpContext.GetSqlAuthUserName(), tmpPwd));

   private RouteValueDictionary GetRedirectRouteValues() {
      RouteValueDictionary tor = new()
      {
         { "area", SqlAuthConsts.SQLAUTHAREA },
         { SqlAuthConsts.URLROUTEPARAMSRV, HttpContext.GetSqlAuthServer() },
         { SqlAuthConsts.URLROUTEPARAMUSR, HttpContext.GetSqlAuthUserName() }
      };

      foreach (KeyValuePair<string, StringValues> q in Request.Query)
      {
         tor.Add(q.Key, q.Value);
      }

      return tor;
   }

   public async Task<IActionResult> OnGetAsync() {
      SqlAuthTempPasswordInfo? tmppwd = await GetTempPasswordInfoAsync();
      if (tmppwd is null)
      {
         return RedirectToPage("connect", GetRedirectRouteValues());
      }

      SqlAuthenticationResult tresult = await ValidateTmpPwdAsync(tmppwd);

      if (!tresult.Success)
      {
         ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
         return Page();
      }
      else
      {
         Input.Databases = await GetDatabasesAsync(tmppwd);
         List<ISqlDatabase> addnotex = [.. Input.Databases];
         addnotex.Add(new SqlDatabase("not_exist"));
         Input.Databases = addnotex;
         return Page();
      }
   }

   public async Task<IActionResult> OnPostAsync([FromQuery] string? returnUrl = null) {
      SqlAuthTempPasswordInfo? tmppwd = await GetTempPasswordInfoAsync();
      if (tmppwd is null)
      {
         return RedirectToPage("connect", GetRedirectRouteValues());
      }

      SqlAuthenticationResult tresult = await ValidateTmpPwdAsync(tmppwd);

      if (!tresult.Success)
      {
         ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
         return Page();
      }
      else if (!ModelState.IsValid)
      {
         return Page();
      }
      else
      {
         //check db-filter
         IEnumerable<ISqlDatabase> dbs = await GetDatabasesAsync(tmppwd);
         //Input.Databases = dbs;
         if (dbs.Count(w => w.Name == Input.DBName) != 1)
         {
            ModelState.AddModelError(string.Empty, $"Database {Input.DBName} not found");
            Input.Databases = dbs;
            return Page();
         }

         //SqlAuthenticationResult authresult = await _sqlConnAuthentication.AuthenticateAsync(Input);
         return Page();
      }

      //SqlAuthenticationResult tresult = await _sqlConnAuthentication.TestAuthenticateAsync(sqlauthparamtemppwd, Input.DBName)
      //   ?? throw new NullReferenceException($"Error in the implementation of {nameof(ISqlAuthService)} returns null");
      //if (tresult.Exception is TemporaryPasswordNotFoundException)
      //{
      //   // redirect to reenter the temporary password
      //   RouteValueDictionary routevalues = new()
      //   {
      //      { "area", SqlAuthConsts.SQLAUTHAREA },
      //      { SqlAuthConsts.URLROUTEPARAMSRV, HttpContext.GetSqlAuthServer() },
      //      { SqlAuthConsts.URLROUTEPARAMUSR, HttpContext.GetSqlAuthUserName() }
      //   };
      //   return RedirectToPage("connect", routevalues);
      //}
      //else if (!tresult.Success)
      //{
      //   ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
      //   return Page();
      //}
      //else
      //{
      //   // authenticate and redirect
      //   //RouteValueDictionary routevalues = new()
      //   //{
      //   //   { "area", SqlAuthConsts.SQLAUTHAREA },
      //   //   { SqlAuthConsts.URLROUTEPARAMSRV, HttpContext.GetSqlAuthServer() },
      //   //   { SqlAuthConsts.URLROUTEPARAMUSR, HttpContext.GetSqlAuthUserName() },
      //   //   { SqlAuthConsts.URLROUTEPARAMDB, Input.DBName }
      //   //};
      //   //return RedirectToPage("connect", routevalues);

      //   SqlAuthenticationResult result = await _sqlConnAuthentication.AuthenticateAsync(Input);

      //   if (!result.Success && result.Exception is not null)
      //   {
      //      ModelState.AddModelError(string.Empty, result.Exception.Message);
      //   }

      //   return result.Success ? Redirect(returnUrl ?? _sqlConnAuthentication.UriEscapedPath)
      //       : Page();
      //}
   }
}

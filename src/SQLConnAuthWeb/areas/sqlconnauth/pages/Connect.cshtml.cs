using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for the Connect page, handling SQL authentication via password or Windows Authentication.
/// </summary>
/// <param name="sqlConnAuthenticationService">The SQL authentication service.</param>
[AllowAnonymous]
[RequireHttps]
public class ConnectModel(ISqlAuthService sqlConnAuthenticationService, SqlAuthAppPaths sqlAuthAppPaths
   , ISqlAuthPwdStore sqlAuthPwdStore) : PageModel
{
   /// <summary>
   /// Gets or sets the input model for password and trust server certificate.
   /// </summary>
   [BindProperty]
   public InputPasswordModel Input { get; set; } = new();

   private readonly ISqlAuthService _sqlConnAuthentication = sqlConnAuthenticationService;

   private readonly SqlAuthAppPaths _sqlAuthAppPaths = sqlAuthAppPaths;

   private readonly ISqlAuthPwdStore _sqlAuthPwdStore = sqlAuthPwdStore;

   /// <summary>
   /// Gets a value indicating whether Windows Authentication is enabled and allowed.
   /// </summary>
   public bool IsWinAuth { get; private set; }

   public string? RouteParamDb => Request.RouteValues[SqlAuthConsts.URLROUTEPARAMDB]?.ToString();

   /// <summary>
   /// Gets the SQL Server name from the authentication context.
   /// </summary>
   public string SQLServer => Request.HttpContext.GetSqlAuthServer();

   /// <summary>
   /// Gets the user name from the authentication context.
   /// </summary>
   public string SqlUserName => Request.HttpContext.GetSqlAuthUserName();

   /// <summary>
   /// Handles GET requests to the Connect page, setting the IsWinAuth property based on the route and options.
   /// </summary>
   public void OnGet()
      => IsWinAuth = SqlUserName == SqlAuthConsts.WINDOWSAUTHENTICATION
         && _sqlConnAuthentication.Options.AllowIntegratedSecurity;

   /// <summary>
   /// Handles POST requests to the Connect page, performing authentication and redirecting or returning the page as appropriate.
   /// </summary>
   /// <param name="returnUrl">The URL to redirect to on successful authentication.</param>
   /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
   public async Task<IActionResult> OnPostAsync([FromRoute] string? sqlauthparamdb
      , [FromQuery] string? returnUrl = null) {
      if (ModelState.IsValid)
      {
         if (_sqlAuthAppPaths.UseDBNameRouting)
         {
            SqlAuthenticationResult testauthenticateresult
               = await _sqlConnAuthentication.TestAuthenticateAsync(Input, sqlauthparamdb);

            if (!testauthenticateresult.Success && testauthenticateresult.Exception is not null)
            {
               ModelState.AddModelError("Password", testauthenticateresult.Exception.Message);
               return Page();
            }
            else
            {
               string tmppwdkey 
                  = await _sqlAuthPwdStore.SetTempPasswordAsync(SqlUserName, SQLServer, Input.Password, Input.TrustServerCertificate);

               RouteValueDictionary routevalues = new() {
                   { "area", SqlAuthConsts.SQLAUTHAREA },
                   { SqlAuthConsts.URLROUTEPARAMSRV, SQLServer },
                   { SqlAuthConsts.URLROUTEPARAMUSR, SqlUserName },
                   { SqlAuthConsts.URLROUTEPARAMTEMPPWD, tmppwdkey }
               };
               if (returnUrl != null)
                  routevalues["returnUrl"] = returnUrl;

               return RedirectToPage("selectdb", routevalues);
            }
         }
         else
         {

            SqlAuthenticationResult result = await _sqlConnAuthentication.AuthenticateAsync(Input);

            if (!result.Success && result.Exception is not null)
            {
               ModelState.AddModelError("Password", result.Exception.Message);
            }

            return result.Success ? Redirect(returnUrl ?? _sqlConnAuthentication.UriEscapedPath)
                : Page();
         }
      }

      return Page();
   }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for the Connect page, handling SQL authentication via password or Windows Authentication.
/// </summary>
/// <param name="sqlConnAuthenticationService">The SQL authentication service.</param>
[AllowAnonymous]
[RequireHttps]
public class ConnectModel(ISqlAuthService sqlConnAuthenticationService) : PageModel
{
   /// <summary>
   /// Gets or sets the input model for password and trust server certificate.
   /// </summary>
   [BindProperty]
   public InputPasswordModel Input { get; set; } = new();

   private readonly ISqlAuthService _sqlConnAuthentication = sqlConnAuthenticationService;

   /// <summary>
   /// Gets a value indicating whether Windows Authentication is enabled and allowed.
   /// </summary>
   public bool IsWinAuth { get; private set; }

   public string RouteParamSrv => Request.RouteValues[SqlAuthConsts.URLROUTEPARAMSRV]?.ToString() ?? string.Empty;

   public string RouteParamUsr => Request.RouteValues[SqlAuthConsts.URLROUTEPARAMUSR]?.ToString() ?? string.Empty;

   public string? RouteParamDb => Request.RouteValues[SqlAuthConsts.URLROUTEPARAMDB]?.ToString();

   /// <summary>
   /// Gets the SQL Server name from the authentication context.
   /// </summary>
   public string SQLServer => _sqlConnAuthentication.SQLServer;

   /// <summary>
   /// Gets the user name from the authentication context.
   /// </summary>
   public string UserName => _sqlConnAuthentication.UserName;

   /// <summary>
   /// Handles GET requests to the Connect page, setting the IsWinAuth property based on the route and options.
   /// </summary>
   public void OnGet()
      => IsWinAuth = RouteParamUsr == SqlAuthConsts.WINDOWSAUTHENTICATION
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
         IsWinAuth = RouteParamUsr == SqlAuthConsts.WINDOWSAUTHENTICATION
            && _sqlConnAuthentication.Options.AllowIntegratedSecurity;

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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for the Disconnect page, handling sign-out for SQL authentication.
/// </summary>
/// <param name="sqlConAuthService">The SQL authentication service.</param>
public class DisconnectModel(ISqlAuthService sqlConAuthService) : PageModel
{
   private readonly ISqlAuthService _sqlconauth = sqlConAuthService;

   /// <summary>
   /// Handles GET requests to the Disconnect page, signing out the user and redirecting if a return URL is provided.
   /// </summary>
   /// <param name="returnUrl">The URL to redirect to after sign-out, or null to return the page.</param>
   /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
   public async Task<IActionResult> OnGetAsync(string? returnUrl = null) {
      await _sqlconauth.SignoutAsync();

      return returnUrl is not null ? Redirect(returnUrl) : Page();
   }
}

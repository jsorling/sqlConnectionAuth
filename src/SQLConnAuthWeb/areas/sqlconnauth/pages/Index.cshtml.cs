using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for the SQL authentication index page, allowing users to enter a SQL Server address and user name.
/// </summary>
[AllowAnonymous]
[RequireHttps]
public class IndexModel(IOptionsMonitor<SqlAuthOptions> options, SqlAuthAppPaths sqlAuthAppPaths) : PageModel
{

   /// <summary>
   /// Gets the SQL authentication options (live-updating).
   /// </summary>
   public SqlAuthOptions SQLAuthOptions => options.CurrentValue;

   /// <summary>
   /// Gets or sets the input model for the SQL Server address and user name.
   /// </summary>
   [BindProperty]
   public required InputServerNameModel Input { get; set; }

   /// <summary>
   /// Handles GET requests to the index page.
   /// </summary>
   /// <returns>The page result.</returns>
   public IActionResult OnGet() => Page();

   /// <summary>
   /// Handles POST requests to the index page, redirecting to the SQL authentication path if the model is valid.
   /// </summary>
   /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
   public IActionResult OnPost() {
      if (ModelState.IsValid)
      {
         if (sqlAuthAppPaths.UseDBNameRouting)
         {
            RouteValueDictionary routevalues = new()
            {
               { "area", SqlAuthConsts.SQLAUTHAREA },
               { SqlAuthConsts.URLROUTEPARAMSRV, Input.SqlServer },
               { SqlAuthConsts.URLROUTEPARAMUSR, Input.UserName }
            };

            return RedirectToPage("connect", routevalues);
         }
         else
         {
            string redir = sqlAuthAppPaths.UriEscapedSqlPath(Input.SqlServer, Input.UserName);
            return Redirect(redir);
         }
      }

      return Page();
   }
}

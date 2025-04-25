using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class IndexModel(IOptions<SqlAuthOptions> options, SqlAuthAppPaths sqlAuthAppPaths) : PageModel
{
   public SqlAuthOptions SQLAuthOptions { get; } = options.Value ?? throw new ArgumentNullException(nameof(options));

   [BindProperty]
   public required InputServerNameModel Input { get; set; }

   public IActionResult OnGet() => Page();

   public IActionResult OnPost()// => ModelState.IsValid
                                //? Redirect(_sqlauth.UriEscapedPath(Input.SqlServer, Input.UserName))
                                //: Page();
   {
      if (ModelState.IsValid)
      {
         string redir = sqlAuthAppPaths.UriEscapedSqlPath(Input.SqlServer, Input.UserName);
         return Redirect(redir);
      }

      return Page();
   }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.helpers;

namespace MockupWeb.Pages.db;

public class IndexModel(ISqlAuthService sqlConAuth) : PageModel
{
   private readonly ISqlAuthService _sqlconauth = sqlConAuth;

   public string? SQLConnectionString => _sqlconauth.GetConnectionString("master");

   public IEnumerable<SqlConnectionHelper.ListDBRes>? DBs;

   public async Task<IActionResult> OnGetAsync([FromRoute] string? db) {
      DBs = await _sqlconauth.GetDBsAsync();
      return Page();
   }
}

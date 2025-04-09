using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace MockupWeb.Pages.db;

public class IndexModel(ISqlConnAuthenticationService sqlConnAuthenticationService) : PageModel
{
   private readonly ISqlConnAuthenticationService _sqlConnAuthenticationService = sqlConnAuthenticationService;

   public string SQLConnectionString => _sqlConnAuthenticationService.SqlConnectionString("master");

   public IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes>? DBs;

   public async Task<IActionResult> OnGetAsync([FromRoute] string? db) {
      DBs = await _sqlConnAuthenticationService.GetDBsAsync();
      return Page();
   }
}

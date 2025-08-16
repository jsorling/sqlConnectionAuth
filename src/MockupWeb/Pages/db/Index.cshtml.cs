using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;
using Sorling.SqlConnAuthWeb.extenstions;

namespace MockupWeb.Pages.db;

public class IndexModel(ISqlAuthDBAccess sqlDBAccess) : PageModel
{
   private readonly ISqlAuthDBAccess _sqldbaccess = sqlDBAccess;

   public string? SQLConnectionString => Request.HttpContext.GetSqlAuthGetConnectionString("master");

   public IEnumerable<ISqlDatabase>? DBs;

   public async Task<IActionResult> OnGetAsync([FromRoute] string? db) {
      DBs = await _sqldbaccess.GetDatabasesAsync(HttpContext.GetSqlAuthConnectionstringProvider());
      return Page();
   }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;

namespace MockupWeb.Pages.db;

public class IndexModel(ISqlAuthDBAccess sqlDBAccess, IHostEnvironment env, SqlAuthAppPaths appPaths, ISqlAuthContext sqlAuthContext) : PageModel
{
   private readonly ISqlAuthDBAccess _sqldbaccess = sqlDBAccess;

   public string AppSettingsPath => Path.Combine(env.ContentRootPath, "appsettings.json");

   public string? SQLConnectionString => sqlAuthContext.GetConnectionString("master");// Request.HttpContext.GetSqlAuthGetConnectionString("master");

   public string SelectedDB => appPaths.UseDBNameRouting ? sqlAuthContext.SqlDBName : "No DB routing";

   public IEnumerable<ISqlDatabase>? DBs;

   public async Task<IActionResult> OnGetAsync([FromRoute] string? db) {
      DBs = await _sqldbaccess.GetDatabasesAsync(sqlAuthContext.ConnectionstringProvider);
      return Page();
   }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;

namespace MockupWeb.Pages.db;

public class IndexModel(ISqlAuthDBAccess sqlDBAccess, IHostEnvironment env, ISqlAuthContext sqlAuthContext) : PageModel
{
   public string AppSettingsPath => Path.Combine(env.ContentRootPath, "appsettings.json");

   public string? SQLConnectionString => sqlAuthContext.GetConnectionString("master");// Request.HttpContext.GetSqlAuthGetConnectionString("master");

   public string SelectedDB => sqlAuthContext.AppPaths.UseDBNameRouting ? sqlAuthContext.SqlDBName ?? "Not on route" : "No DB routing";

   public IEnumerable<ISqlDatabase>? DBs;

   public async Task<IActionResult> OnGetAsync() {
      DBs = await sqlDBAccess.GetDatabasesAsync(sqlAuthContext.ConnectionstringProvider);
      return Page();
   }
}

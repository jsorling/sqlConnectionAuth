using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb;
using Sorling.SqlConnAuthWeb.razor;

namespace MockupWeb.Pages.db.import;

public class IndexModel : PageModel
{
   public IActionResult OnGet() {
      //RouteValueDictionary rv = new() {
      //   ["area"] = null
      //};

      //// Preserve SQL auth route parameters so the rewritten route can be generated
      //string? srv = RouteData.Values[SqlAuthConsts.URLROUTEPARAMSRV] as string;
      //string? usr = RouteData.Values[SqlAuthConsts.URLROUTEPARAMUSR] as string;
      //string? db = RouteData.Values[SqlAuthConsts.URLROUTEPARAMDB] as string;

      //if (!string.IsNullOrEmpty(srv))
      //   rv[SqlAuthConsts.URLROUTEPARAMSRV] = srv;
      //if (!string.IsNullOrEmpty(usr))
      //   rv[SqlAuthConsts.URLROUTEPARAMUSR] = usr;
      //if (!string.IsNullOrEmpty(db))
      //   rv[SqlAuthConsts.URLROUTEPARAMDB] = db;

      //// Use relative page path to target sibling page explicitly
      //return RedirectToPage("./upload", rv);
      //return RedirectToPage("./upload");
      //return this.RedirectToPageWithSqlAuth("./upload");
      return this.RedirectToPage("./upload");
   }
}

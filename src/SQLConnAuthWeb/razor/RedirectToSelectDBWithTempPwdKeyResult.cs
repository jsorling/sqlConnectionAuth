using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.passwords;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// Redirects to the SelectDB page with a generated temporary password key in the route.
/// </summary>
public class RedirectToSelectDBWithTempPwdKeyResult : ActionResult
{
   internal RedirectToSelectDBWithTempPwdKeyResult(ISqlAuthContext context
      , ISqlAuthPwdStore pwdStore
      , IUrlHelper urlHelper) {
      _context = context;
      _pwdStore = pwdStore;
      _urlHelper = urlHelper;      
   }

   private readonly ISqlAuthContext _context;

   private readonly ISqlAuthPwdStore _pwdStore;

   private readonly IUrlHelper _urlHelper;

   public override async Task ExecuteResultAsync(ActionContext context) {
      SqlAuthStoredSecrets storedsecrets = _context.StoredSecrets ?? throw new ApplicationException("Stored secrets not set");
      string temppwdkey = await _pwdStore.SetTempPasswordAsync(
          _context.SqlUserName,
          _context.SqlServer,
          storedsecrets.Password,
          storedsecrets.TrustServerCertificate);

      RouteValueDictionary routevalues = new() {
             { "area", SqlAuthConsts.SQLAUTHAREA },
             { SqlAuthConsts.URLROUTEPARAMSRV, _context.SqlServer },
             { SqlAuthConsts.URLROUTEPARAMUSR, _context.SqlUserName },
             { SqlAuthConsts.URLROUTEPARAMTEMPPWD, temppwdkey }
      };

      string url = _urlHelper.Page("/selectdb", routevalues)
         ?? throw new ApplicationException("Failed to generate SelectDB URL");

      context.HttpContext.Response.Redirect(url);
   }
}

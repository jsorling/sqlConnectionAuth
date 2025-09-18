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
   internal RedirectToSelectDBWithTempPwdKeyResult(ISqlAuthContext sqlAuthContext
      , ISqlAuthPwdStore pwdStore
      , IUrlHelper urlHelper
      , string? returnUrl) {
      _context = sqlAuthContext;
      _pwdStore = pwdStore;
      _urlHelper = urlHelper;
      _returnUrl = returnUrl;

      _sqlUserName = sqlAuthContext.SqlUserName
         ?? throw new ArgumentNullException(nameof(sqlAuthContext), "No active sqlauth context");
      _sqlServer = sqlAuthContext.SqlServer
         ?? throw new ArgumentNullException(nameof(sqlAuthContext), "No active sqlauth context");
   }

   private readonly ISqlAuthContext _context;

   private readonly ISqlAuthPwdStore _pwdStore;

   private readonly IUrlHelper _urlHelper;

   private readonly string? _returnUrl;

   private readonly string _sqlUserName;

   private readonly string _sqlServer;

   public override async Task ExecuteResultAsync(ActionContext context) {
      SqlAuthStoredSecrets storedsecrets = _context.StoredSecrets ?? throw new ApplicationException("Stored secrets not set");
      string temppwdkey = await _pwdStore.SetTempPasswordAsync(
          _sqlUserName,
          _sqlServer,
          storedsecrets.Password,
          storedsecrets.TrustServerCertificate);

      RouteValueDictionary routevalues = new() {
             { "area", SqlAuthConsts.SQLAUTHAREA },
             { SqlAuthConsts.URLROUTEPARAMSRV, _sqlServer },
             { SqlAuthConsts.URLROUTEPARAMUSR, _sqlUserName },
             { SqlAuthConsts.URLROUTEPARAMTEMPPWD, temppwdkey }
      };

      if (!string.IsNullOrEmpty(_returnUrl))
      {
         routevalues.Add("returnUrl", _returnUrl);
      }

      string url = _urlHelper.Page("/selectdb", routevalues)
         ?? throw new ApplicationException("Failed to generate SelectDB URL");

      context.HttpContext.Response.Redirect(url);
   }
}

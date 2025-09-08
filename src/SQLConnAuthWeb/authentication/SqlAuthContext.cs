using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.razor;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <inheritdoc cref="ISqlAuthContext"/>
public class SqlAuthContext(IUrlHelper urlHelper, ISqlAuthPwdStore sqlAuthPwdStore, SqlAuthAppPaths appPaths) : ISqlAuthContext
{
   private RouteValueDictionary RouteValuesSrvUsr => new()
      {
         { "area", null },
         { SqlAuthConsts.URLROUTEPARAMSRV, SqlServer },
         { SqlAuthConsts.URLROUTEPARAMUSR, SqlUserName }
      };

   /// <inheritdoc/>
   public string SqlServer => urlHelper.ActionContext.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMSRV]
      as string ?? throw new ApplicationException("SQL server cannot be null or empty on route.");

   /// <inheritdoc/>
   public string SqlUserName => urlHelper.ActionContext.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMUSR] as string
      ?? throw new ApplicationException("SQL username cannot be null or empty on route.");

   /// <inheritdoc/>
   public string? SqlDBName => urlHelper.ActionContext.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMDB] as string;

   /// <inheritdoc/>
   public SqlAuthStoredSecrets? StoredSecrets
      => urlHelper.ActionContext.HttpContext.Items[typeof(SqlAuthStoredSecrets)] as SqlAuthStoredSecrets;

   /// <inheritdoc/>
   public SqlAuthConnectionstringProvider ConnectionstringProvider
      => StoredSecrets is SqlAuthStoredSecrets storedsecrets
         ? new SqlAuthConnectionstringProvider(SqlServer, SqlUserName, storedsecrets)
         : throw new ApplicationException("SQL connection string provider cannot be created without stored secrets.");

   /// <inheritdoc/>
   public string? GetConnectionString(string? database = null)
      => ConnectionstringProvider.ConnectionString(database ?? SqlDBName);

   /// <inheritdoc/>
   public string? TempPwdKeyFromQuery
      => urlHelper.ActionContext.HttpContext.Request.Query[SqlAuthConsts.QUERYPARAMTMPPWDKEY].FirstOrDefault();

   /// <inheritdoc/>
   public string? TempPwdKey
      => urlHelper.ActionContext.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMTEMPPWD] as string;

   /// <inheritdoc/>
   public RedirectToSelectDBWithTempPwdKeyResult GetRedirectToSelectDBActionResult() =>
      new(this, sqlAuthPwdStore, urlHelper);

   /// <inheritdoc/>
   public SqlAuthAppPaths AppPaths => appPaths;
}

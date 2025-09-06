using Microsoft.AspNetCore.Mvc;
using Sorling.SqlConnAuthWeb.authentication.passwords;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <inheritdoc cref="ISqlAuthContext"/>
public class SqlAuthContext(IUrlHelper urlHelper) : ISqlAuthContext
{
   /// <inheritdoc/>
   public string SelectDBUrl => urlHelper.Page($"/{SqlAuthConsts.SQLAUTHAREA}/selectdb")
      ?? throw new ApplicationException($"Failed to get selectdb URL");

   /// <inheritdoc/>
   public string SqlServer => urlHelper.ActionContext.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMSRV]
      as string ?? throw new ApplicationException("SQL server cannot be null or empty on route.");

   /// <inheritdoc/>
   public string SqlUserName => urlHelper.ActionContext.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMUSR] as string
         ?? throw new ApplicationException("SQL username cannot be null or empty on route.");

   /// <inheritdoc/>
   public string SqlDBName => urlHelper.ActionContext.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMDB] as string
         ?? throw new ApplicationException("Sql database name not defined on route");

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
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.helpers;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Provides SQL authentication services, including authentication, sign-out, connection string management, and database listing for Razor Pages applications.
/// </summary>
/// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
/// <param name="ruleValidator">Validator for SQL authentication rules.</param>
/// <param name="pwdStore">Store for SQL authentication secrets.</param>
/// <param name="options">Options for SQL authentication configuration.</param>
/// <param name="sqlAuthAppPaths">Application path configuration for SQL authentication.</param>
public class SqlAuthService(IHttpContextAccessor httpContextAccessor, ISqlAuthRuleValidator ruleValidator
   , ISqlAuthPwdStore pwdStore, IOptions<SqlAuthOptions> options, SqlAuthAppPaths sqlAuthAppPaths) : ISqlAuthService
{
   private readonly HttpContext _httpContext = httpContextAccessor?.HttpContext
       ?? throw new ArgumentNullException(nameof(httpContextAccessor));

   private readonly ISqlAuthRuleValidator _ruleValidator = ruleValidator
       ?? throw new ArgumentNullException(nameof(ruleValidator));

   private readonly ISqlAuthPwdStore _pwdStore = pwdStore
       ?? throw new ArgumentNullException(nameof(pwdStore));

   private readonly SqlAuthAppPaths _sqlAuthAppPaths = sqlAuthAppPaths
       ?? throw new ArgumentNullException(nameof(sqlAuthAppPaths));

   /// <inheritdoc/>
   public SqlAuthOptions Options { get; } = options?.Value ?? throw new ArgumentNullException(nameof(options));

   /// <inheritdoc/>
   public string SQLServer => _httpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMSRV] as string
       ?? throw new ApplicationException("SQL server cannot be null or empty on route.");

   /// <inheritdoc/>
   public string UserName => _httpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMUSR] as string
       ?? throw new ApplicationException("SQL username cannot be null or empty on route.");

   /// <inheritdoc/>
   public string UriEscapedPath => _sqlAuthAppPaths.UriEscapedSqlPath(SQLServer, UserName);

   /// <inheritdoc/>
   public SqlAuthStoredSecrets? SqlAuthStoredSecrets
      => _httpContext.Items[typeof(SqlAuthStoredSecrets)] as SqlAuthStoredSecrets;

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> AuthenticateAsync(SQLAuthenticateRequest request) {
      SqlAuthRuleValidationResult validationresult = await _ruleValidator.ValidateAsync(
          new(SQLServer, UserName, request.Password, request.TrustServerCertificate));
      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;
      if (storedsecrets is null)
         return new(false, validationresult.Exception, null);

      SqlAuthenticationResult result = await SqlConnectionHelper.TryConnectWithResultAsync(
          new(SQLServer, UserName, storedsecrets));
      if (result.Success)
      {
         await _httpContext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

         List<Claim>? claims = [
             new Claim(SqlAuthConsts.CLAIMSQLSERVER, SQLServer, ClaimValueTypes.String
                    , SqlAuthConsts.SQLAUTHSCHEME),
                new Claim(SqlAuthConsts.CLAIMSQLUSERNAME, UserName, ClaimValueTypes.String
                    , SqlAuthConsts.SQLAUTHSCHEME),
                new Claim(ClaimTypes.Name, $"{UserName}@{SQLServer}", ClaimValueTypes.String
                    , SqlAuthConsts.SQLAUTHSCHEME),
                new Claim(SqlAuthConsts.CLAIMSQLPASSWORDREF, await _pwdStore.StoreAsync(storedsecrets)
                    , ClaimValueTypes.String, SqlAuthConsts.SQLAUTHSCHEME)
         ];

         ClaimsIdentity claimsidentity = new(claims, SqlAuthConsts.SQLAUTHSCHEME);
         await _httpContext.SignInAsync(SqlAuthConsts.SQLAUTHSCHEME, new ClaimsPrincipal(claimsidentity));
      }

      return result;
   }

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> TestAuthenticateAsync(SqlAuthTempPasswordInfo sqlAuthTempPasswordInfo, string? dbName) {
      ArgumentNullException.ThrowIfNull(sqlAuthTempPasswordInfo, nameof(sqlAuthTempPasswordInfo));

      SqlAuthRuleValidationResult validationresult = await _ruleValidator.ValidateAsync(
          new(SQLServer, UserName, sqlAuthTempPasswordInfo.Password, sqlAuthTempPasswordInfo.TrustServerCertificate));

      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;
      return storedsecrets is null
         ? new(false, validationresult.Exception, null)
         : await SqlConnectionHelper.TryConnectWithResultAsync(
          new(SQLServer, UserName, storedsecrets));
   }

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> TestAuthenticateAsync(string key, string? dbName){
      SqlAuthTempPasswordInfo? temppasswordinfo = await _pwdStore.PeekTempPasswordAsync(key);

      return temppasswordinfo is null
         ? new(false, new ApplicationException("Temporary password not found."), null)
         : await TestAuthenticateAsync(temppasswordinfo, dbName);
   }

   /// <inheritdoc/>
   public async Task SignoutAsync()
       => await _httpContext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

   /// <inheritdoc/>
   public string? GetConnectionString(string? database = null) {
      if (SqlAuthStoredSecrets is SqlAuthStoredSecrets storedsecrets)
      {
         SqlAuthConnectionstringProvider sca = new(SQLServer, UserName, storedsecrets);
         return sca.ConnectionString(database);
      }

      return null;
   }

   /// <inheritdoc/>
   public async Task<IEnumerable<SqlConnectionHelper.DBName>> GetDBsAsync() {
      if (SqlAuthStoredSecrets is SqlAuthStoredSecrets storedsecrets)
      {
         SqlAuthConnectionstringProvider sca = new(SQLServer, UserName, storedsecrets);
         return await SqlConnectionHelper.GetDbsAsync(sca);
      }

      return [];
   }
}

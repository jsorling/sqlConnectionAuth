using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.exceptions;
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
public class SqlAuthService(IHttpContextAccessor httpContextAccessor
   , ISqlAuthRuleValidator ruleValidator
   , ISqlAuthPwdStore pwdStore
   , SqlAuthAppPaths sqlAuthAppPaths
   , ISqlAuthContext sqlAuthContext) : ISqlAuthService
{
   private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

   private readonly ISqlAuthRuleValidator _ruleValidator = ruleValidator ?? throw new ArgumentNullException(nameof(ruleValidator));

   private readonly ISqlAuthPwdStore _pwdStore = pwdStore ?? throw new ArgumentNullException(nameof(pwdStore));

   private readonly SqlAuthAppPaths _sqlAuthAppPaths = sqlAuthAppPaths ?? throw new ArgumentNullException(nameof(sqlAuthAppPaths));

   /// <inheritdoc/>
   public string UriEscapedPath
   => _sqlAuthAppPaths.UriEscapedSqlPath(
         sqlAuthContext.SqlServer ?? string.Empty,
         sqlAuthContext.SqlUserName ?? string.Empty);

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> AuthenticateAsync(SQLAuthenticateRequest request) {
      HttpContext httpcontext = _httpContextAccessor.HttpContext ?? throw new NullReferenceException(nameof(_httpContextAccessor));

      SqlAuthRuleValidationResult validationresult = await _ruleValidator.ValidateConnectionAsync(
          new(sqlAuthContext.SqlServer, sqlAuthContext.SqlUserName, request.Password, request.TrustServerCertificate)
          , request.DBName);
      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;

      if (storedsecrets is null)
         return new(false, validationresult.Exception, null);

      if (sqlAuthAppPaths.UseDBNameRouting && request.NoDataBaseFilter == false)
      {
         if (storedsecrets.DBName is null)
         {
            return new(false, new ApplicationException("Database name is null"), null);
         }

         if (!await _ruleValidator.ValidateDatabaseAsync(storedsecrets.DBName))
         {
            return new(false, new ApplicationException("Database name not found in database filter"), null);
         }
      }

      SqlAuthenticationResult result = await SqlConnectionHelper.TryConnectWithResultAsync(
          new(sqlAuthContext.SqlServer, sqlAuthContext.SqlUserName, storedsecrets));
      if (result.Success)
      {
         await httpcontext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

         List<Claim>? claims = [
            new Claim(SqlAuthConsts.CLAIMSQLSERVER, sqlAuthContext.SqlServer, ClaimValueTypes.String
               , SqlAuthConsts.SQLAUTHSCHEME),
            new Claim(SqlAuthConsts.CLAIMSQLUSERNAME, sqlAuthContext.SqlUserName, ClaimValueTypes.String
               , SqlAuthConsts.SQLAUTHSCHEME),
            new Claim(ClaimTypes.Name, $"{sqlAuthContext.SqlUserName}@{sqlAuthContext.SqlServer}", ClaimValueTypes.String
               , SqlAuthConsts.SQLAUTHSCHEME),
            new Claim(SqlAuthConsts.CLAIMSQLPASSWORDREF, await _pwdStore.StoreAsync(storedsecrets)
               , ClaimValueTypes.String, SqlAuthConsts.SQLAUTHSCHEME)
         ];

         ClaimsIdentity claimsidentity = new(claims, SqlAuthConsts.SQLAUTHSCHEME);
         await httpcontext.SignInAsync(SqlAuthConsts.SQLAUTHSCHEME, new ClaimsPrincipal(claimsidentity));
      }

      return result;
   }

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> TestAuthenticateAsync(SqlAuthTempPasswordInfo sqlAuthTempPasswordInfo, string? dbName) {
      ArgumentNullException.ThrowIfNull(sqlAuthTempPasswordInfo, nameof(sqlAuthTempPasswordInfo));

      SqlAuthRuleValidationResult validationresult = await _ruleValidator.ValidateConnectionAsync(
          new(sqlAuthContext.SqlServer, sqlAuthContext.SqlUserName, sqlAuthTempPasswordInfo.Password
            , sqlAuthTempPasswordInfo.TrustServerCertificate), dbName);

      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;
      return storedsecrets is null
         ? new(false, validationresult.Exception, null)
         : await SqlConnectionHelper.TryConnectWithResultAsync(
            new(sqlAuthContext.SqlServer, sqlAuthContext.SqlUserName, storedsecrets));
   }

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> TestAuthenticateAsync(string key, string? dbName) {
      SqlAuthTempPasswordInfo? temppasswordinfo = await _pwdStore.PeekTempPasswordAsync(key);

      return temppasswordinfo is null
         ? new(false, new TemporaryPasswordNotFoundException(), null)
         : await TestAuthenticateAsync(temppasswordinfo, dbName);
   }
}

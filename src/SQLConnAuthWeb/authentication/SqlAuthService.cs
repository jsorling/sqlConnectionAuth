using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.extenstions;
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
      , ISqlAuthPwdStore pwdStore, IOptionsMonitor<SqlAuthOptions> optionsMonitor, SqlAuthAppPaths sqlAuthAppPaths) : ISqlAuthService
{
   private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
   private readonly ISqlAuthRuleValidator _ruleValidator = ruleValidator ?? throw new ArgumentNullException(nameof(ruleValidator));
   private readonly ISqlAuthPwdStore _pwdStore = pwdStore ?? throw new ArgumentNullException(nameof(pwdStore));
   private readonly SqlAuthAppPaths _sqlAuthAppPaths = sqlAuthAppPaths ?? throw new ArgumentNullException(nameof(sqlAuthAppPaths));
   private readonly IOptionsMonitor<SqlAuthOptions> _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));

   /// <inheritdoc/>
   public SqlAuthOptions Options => _optionsMonitor.CurrentValue;

   /// <inheritdoc/>
   public string UriEscapedPath
   => _sqlAuthAppPaths.UriEscapedSqlPath(
         _httpContextAccessor.HttpContext?.SqlAuthServer() ?? string.Empty,
         _httpContextAccessor.HttpContext?.SqlAuthUserName() ?? string.Empty);

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> AuthenticateAsync(SQLAuthenticateRequest request) {
      HttpContext httpcontext = _httpContextAccessor.HttpContext ?? throw new NullReferenceException(nameof(_httpContextAccessor));
      SqlAuthRuleValidationResult validationresult = await _ruleValidator.ValidateAsync(
          new(httpcontext.SqlAuthServer(), httpcontext.SqlAuthUserName(), request.Password, request.TrustServerCertificate));
      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;
      if (storedsecrets is null)
         return new(false, validationresult.Exception, null);

      SqlAuthenticationResult result = await SqlConnectionHelper.TryConnectWithResultAsync(
          new(httpcontext.SqlAuthServer(), httpcontext.SqlAuthUserName(), storedsecrets));
      if (result.Success)
      {
         await httpcontext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

         List<Claim>? claims = [
            new Claim(SqlAuthConsts.CLAIMSQLSERVER, httpcontext.SqlAuthServer(), ClaimValueTypes.String
               , SqlAuthConsts.SQLAUTHSCHEME),
            new Claim(SqlAuthConsts.CLAIMSQLUSERNAME, httpcontext.SqlAuthUserName(), ClaimValueTypes.String
               , SqlAuthConsts.SQLAUTHSCHEME),
            new Claim(ClaimTypes.Name, $"{httpcontext.SqlAuthUserName()}@{httpcontext.SqlAuthServer()}", ClaimValueTypes.String
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

      HttpContext httpcontext = _httpContextAccessor.HttpContext ?? throw new NullReferenceException(nameof(_httpContextAccessor));

      SqlAuthRuleValidationResult validationresult = await _ruleValidator.ValidateAsync(
          new(httpcontext.SqlAuthServer(), httpcontext.SqlAuthUserName(), sqlAuthTempPasswordInfo.Password, sqlAuthTempPasswordInfo.TrustServerCertificate));

      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;
      return storedsecrets is null
         ? new(false, validationresult.Exception, null)
         : await SqlConnectionHelper.TryConnectWithResultAsync(
          new(httpcontext.SqlAuthServer(), httpcontext.SqlAuthUserName(), storedsecrets));
   }

   /// <inheritdoc/>
   public async Task<SqlAuthenticationResult> TestAuthenticateAsync(string key, string? dbName) {
      SqlAuthTempPasswordInfo? temppasswordinfo = await _pwdStore.PeekTempPasswordAsync(key);

      return temppasswordinfo is null
         ? new(false, new ApplicationException("Temporary password not found."), null)
         : await TestAuthenticateAsync(temppasswordinfo, dbName);
   }

   /// <inheritdoc/>
   public async Task<IEnumerable<SqlConnectionHelper.DBName>> GetDBsAsync() {
      HttpContext? httpcontext = _httpContextAccessor.HttpContext;
      if (httpcontext != null && httpcontext.SqlAuthStoredSecrets() is SqlAuthStoredSecrets storedsecrets)
      {
         SqlAuthConnectionstringProvider sca = new(httpcontext.SqlAuthServer(), httpcontext.SqlAuthUserName(), storedsecrets);
         return await SqlConnectionHelper.GetDbsAsync(sca);
      }

      return [];
   }
}

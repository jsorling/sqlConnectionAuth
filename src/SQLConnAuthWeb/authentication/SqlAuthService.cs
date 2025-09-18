using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.exceptions;
using Sorling.SqlConnAuthWeb.helpers;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Provides SQL authentication services for Razor Pages applications, including authentication, sign-out, connection string management, and database listing.
/// </summary>
/// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
/// <param name="ruleValidator">Validator for SQL authentication rules.</param>
/// <param name="pwdStore">Store for SQL authentication secrets.</param>
/// <param name="sqlAuthContext">Authentication context providing connection and user details.</param>
public class SqlAuthService(
    IHttpContextAccessor httpContextAccessor,
    ISqlAuthRuleValidator ruleValidator,
    ISqlAuthPwdStore pwdStore,
    ISqlAuthContext sqlAuthContext
) : ISqlAuthService
{
   private readonly string _sqlserver = sqlAuthContext.SqlServer 
      ?? throw new ArgumentNullException(nameof(sqlAuthContext.SqlServer));

   private readonly string _sqlusername = sqlAuthContext.SqlUserName 
      ?? throw new ArgumentNullException(nameof(sqlAuthContext.SqlUserName));

   /// <summary>
   /// Authenticates a user using the provided SQL authentication request. On success, signs out any existing authentication and signs in with new claims.
   /// </summary>
   /// <param name="request">The authentication request containing credentials and connection details.</param>
   /// <returns>
   /// A <see cref="Task{SqlAuthenticationResult}"/> representing the asynchronous operation. The result contains the authentication outcome.
   /// </returns>
   public async Task<SqlAuthenticationResult> AuthenticateAsync(SQLAuthenticateRequest request) {
      HttpContext httpcontext = httpContextAccessor.HttpContext ?? throw new NullReferenceException(nameof(httpContextAccessor));

      SqlAuthRuleValidationResult validationresult = await ruleValidator.ValidateConnectionAsync(
          new(_sqlserver, _sqlusername, request.Password, request.TrustServerCertificate),
          request.DBName);
      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;

      if (storedsecrets is null)
         return new(false, validationresult.Exception, null);

      if (sqlAuthContext.AppPaths.UseDBNameRouting && request.NoDataBaseFilter == false)
      {
         if (storedsecrets.DBName is null)
         {
            return new(false, new ApplicationException("Database name is null"), null);
         }

         if (!await ruleValidator.ValidateDatabaseAsync(storedsecrets.DBName))
         {
            return new(false, new ApplicationException("Database name not found in database filter"), null);
         }
      }

      SqlAuthenticationResult result = await SqlConnectionHelper.TryConnectWithResultAsync(
          new(_sqlserver, _sqlusername, storedsecrets));
      if (result.Success)
      {
         // Sign out any existing authentication and sign in with new claims
         await httpcontext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

         List<Claim>? claims = [
             new Claim(SqlAuthConsts.CLAIMSQLSERVER, _sqlserver, ClaimValueTypes.String, SqlAuthConsts.SQLAUTHSCHEME),
                new Claim(SqlAuthConsts.CLAIMSQLUSERNAME, _sqlusername, ClaimValueTypes.String, SqlAuthConsts.SQLAUTHSCHEME),
                new Claim(ClaimTypes.Name, $"{_sqlusername}@{_sqlserver}", ClaimValueTypes.String, SqlAuthConsts.SQLAUTHSCHEME),
                new Claim(SqlAuthConsts.CLAIMSQLPASSWORDREF, await pwdStore.StoreAsync(storedsecrets), ClaimValueTypes.String, SqlAuthConsts.SQLAUTHSCHEME)
         ];

         ClaimsIdentity claimsidentity = new(claims, SqlAuthConsts.SQLAUTHSCHEME);
         await httpcontext.SignInAsync(SqlAuthConsts.SQLAUTHSCHEME, new ClaimsPrincipal(claimsidentity));
      }

      return result;
   }

   /// <summary>
   /// Tests SQL authentication using a temporary password and optional database name, without affecting the current authentication state.
   /// </summary>
   /// <param name="sqlAuthTempPasswordInfo">The temporary password information for SQL authentication.</param>
   /// <param name="dbName">The name of the database to test authentication against, or null for the default database.</param>
   /// <returns>
   /// A <see cref="Task{SqlAuthenticationResult}"/> representing the asynchronous operation. The result contains the authentication outcome.
   /// </returns>
   public async Task<SqlAuthenticationResult> TestAuthenticateAsync(SqlAuthTempPasswordInfo sqlAuthTempPasswordInfo, string? dbName) {
      ArgumentNullException.ThrowIfNull(sqlAuthTempPasswordInfo, nameof(sqlAuthTempPasswordInfo));

      SqlAuthRuleValidationResult validationresult = await ruleValidator.ValidateConnectionAsync(
          new(_sqlserver, _sqlusername, sqlAuthTempPasswordInfo.Password
            , sqlAuthTempPasswordInfo.TrustServerCertificate), dbName);

      SqlAuthStoredSecrets? storedsecrets = validationresult.StoredSecrets;
      return storedsecrets is null
          ? new(false, validationresult.Exception, null)
          : await SqlConnectionHelper.TryConnectWithResultAsync(
              new(_sqlserver, _sqlusername, storedsecrets));
   }

   /// <summary>
   /// Tests SQL authentication using a specified key and optional database name, without affecting the current authentication state. The key is only peeked and not removed.
   /// </summary>
   /// <param name="key">The key used to retrieve temporary authentication information. The key is not removed (peek only).</param>
   /// <param name="dbName">The name of the database to test authentication against, or null for the default database.</param>
   /// <returns>
   /// A <see cref="Task{SqlAuthenticationResult}"/> representing the asynchronous operation. The result contains the authentication outcome.
   /// </returns>
   public async Task<SqlAuthenticationResult> TestAuthenticateAsync(string key, string? dbName) {
      SqlAuthTempPasswordInfo? temppasswordinfo = await pwdStore.PeekTempPasswordAsync(key);

      return temppasswordinfo is null
          ? new(false, new TemporaryPasswordNotFoundException(), null)
          : await TestAuthenticateAsync(temppasswordinfo, dbName);
   }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.extenstions;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Handles events for SQL authentication cookies, including signing in, validating principals, signing out, and redirecting to login.
/// </summary>
/// <param name="sqlConnAuthPwdStore">The password store for SQL authentication secrets.</param>
/// <param name="ruleValidator">The rule validator for SQL authentication rules.</param>
/// <param name="sqlAuthAppPaths">The application path configuration for SQL authentication.</param>
public class SqlAuthCookieEvents(ISqlAuthPwdStore sqlConnAuthPwdStore
   , ISqlAuthRuleValidator ruleValidator
   , SqlAuthAppPaths sqlAuthAppPaths) : CookieAuthenticationEvents
{
   /// <summary>
   /// Called during the sign-in process to set cookie options such as path and SameSite mode.
   /// </summary>
   /// <param name="context">The context for the signing-in event.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public override Task SigningIn(CookieSigningInContext context) {
      string? server = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLSERVER)?.Value;
      string? user = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLUSERNAME)?.Value;

      context.CookieOptions.IsEssential = true;
      context.CookieOptions.Path
          = $"/{sqlAuthAppPaths.Root.Trim('/')}/{Uri.EscapeDataString(server!)}/{Uri.EscapeDataString(user!)}".Replace("//", "/");
      context.CookieOptions.SameSite = SameSiteMode.Strict;

      return base.SigningIn(context);
   }

   // Helper to extract claims from the principal
   private static (string? server, string? user, string? passwordref) ExtractClaims(ClaimsPrincipal? principal)
      => (
         principal?.FindFirst(SqlAuthConsts.CLAIMSQLSERVER)?.Value,
         principal?.FindFirst(SqlAuthConsts.CLAIMSQLUSERNAME)?.Value,
         principal?.FindFirst(SqlAuthConsts.CLAIMSQLPASSWORDREF)?.Value
      );

   // Helper to retrieve stored secrets
   private async Task<SqlAuthStoredSecrets?> GetStoredSecretsAsync(SqlAuthCookieClaims? scc) => scc?.SecretStoreKey is not null
         ? await sqlConnAuthPwdStore.RetrieveAsync(scc.SecretStoreKey)
         : null;

   // Helper to check DB name filter
   private bool ShouldRejectForDbNameFilter(string? dbname, SqlAuthStoredSecrets storedsecrets)
      => sqlAuthAppPaths.UseDBNameRouting && (string.IsNullOrEmpty(dbname) || dbname != storedsecrets.DBName);

   // Helper to revalidate and renew secrets if needed
   private async Task<SqlAuthStoredSecrets?> RevalidateSecretsIfNeededAsync(string server, string user, SqlAuthStoredSecrets storedsecrets, SqlAuthCookieClaims scc) {
      if (storedsecrets.RuleReValidationAfter.HasValue && storedsecrets.RuleReValidationAfter.Value < DateTime.UtcNow)
      {
         SqlAuthRuleValidationResult validationresult = await ruleValidator
            .ValidateConnectionAsync(new(server, user, storedsecrets.Password, storedsecrets.TrustServerCertificate)
               , storedsecrets.DBName);
         if (validationresult.Exception is not null || validationresult.StoredSecrets is null)
         {
            return null;
         }

         await sqlConnAuthPwdStore.RenewAsync(scc.SecretStoreKey, validationresult.StoredSecrets);
         return validationresult.StoredSecrets;
      }

      return storedsecrets;
   }

   // Helper to set secrets in HttpContext
   private static void SetSecretsInHttpContext(HttpContext httpContext, SqlAuthStoredSecrets storedsecrets)
      => httpContext.Items[typeof(SqlAuthStoredSecrets)] = storedsecrets;

   /// <summary>
   /// Called to validate the principal on each request. Handles revalidation and secret renewal if necessary.
   /// </summary>
   /// <param name="context">The context for the principal validation event.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public override async Task ValidatePrincipal(CookieValidatePrincipalContext context) {
      (string? server, string? user, string? passwordref) = ExtractClaims(context.Principal);
      if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(passwordref))
      {
         context.RejectPrincipal();
         return;
      }

      SqlAuthCookieClaims? scc = context.Principal?.Identities.SqlConnAuthCookieClaims();
      SqlAuthStoredSecrets? storedsecrets = await GetStoredSecretsAsync(scc);
      if (storedsecrets is null)
      {
         context.RejectPrincipal();
         return;
      }

      if (sqlAuthAppPaths.UseDBNameRouting)
      {
         string dbname = context.HttpContext.GetSqlAuthDBName();
         if (ShouldRejectForDbNameFilter(dbname, storedsecrets))
         {
            context.RejectPrincipal();
            context.HttpContext.Response.Redirect(
               sqlAuthAppPaths.UriEscapedSqlPath(server, user)
               + (string.IsNullOrWhiteSpace(dbname) ? "" : $"/{Uri.EscapeDataString(dbname)}"));
            return;
         }
      }

      if (scc is not null)
      {
         SqlAuthStoredSecrets? revalidatedsecrets = await RevalidateSecretsIfNeededAsync(server, user, storedsecrets, scc);
         if (revalidatedsecrets is null)
         {
            context.RejectPrincipal();
            return;
         }

         storedsecrets = revalidatedsecrets;
      }

      SetSecretsInHttpContext(context.HttpContext, storedsecrets);
   }

   /// <summary>
   /// Called during the sign-out process.
   /// </summary>
   /// <param name="context">The context for the signing-out event.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public override Task SigningOut(CookieSigningOutContext context) => base.SigningOut(context);

   /// <summary>
   /// Called when redirecting to the login page. Modifies the redirect URI to include server and user information if available.
   /// </summary>
   /// <param name="context">The context for the redirect to login event.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context) {
      string? dbsrv = context.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMSRV] as string;
      string? username = context.HttpContext.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMUSR] as string;

      if (!string.IsNullOrEmpty(dbsrv) && !string.IsNullOrEmpty(username))
      {
         Uri uri = new(context.RedirectUri);
         string? newredirecturi = uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port, UriFormat.UriEscaped)
             + $"{context.Options.LoginPath}/{Uri.EscapeDataString(dbsrv)}/{Uri.EscapeDataString(username)}?"
             + uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);

         context.Response.Redirect(newredirecturi);
         return Task.CompletedTask;
      }

      return base.RedirectToLogin(context);
   }
}

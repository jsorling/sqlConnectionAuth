using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.extenstions;

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
          = $"/{sqlAuthAppPaths.Root.Trim('/')}/{Uri.EscapeDataString(server!)}/{Uri.EscapeDataString(user!)}";
      context.CookieOptions.SameSite = SameSiteMode.Strict;

      return base.SigningIn(context);
   }

   /// <summary>
   /// Called to validate the principal on each request. Handles revalidation and secret renewal if necessary.
   /// </summary>
   /// <param name="context">The context for the principal validation event.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   public override async Task ValidatePrincipal(CookieValidatePrincipalContext context) {
      string? server = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLSERVER)?.Value;
      string? user = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLUSERNAME)?.Value;
      string? passwordref = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLPASSWORDREF)?.Value;
      string? dbname = context.Principal?.FindFirst(SqlAuthConsts.URLROUTEPARAMDB)?.Value;

      if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(passwordref))
      {
         context.RejectPrincipal();
      }
      else
      {
         SqlAuthCookieClaims? scc = context.Principal?.Identities.SqlConnAuthCookieClaims();
         SqlAuthStoredSecrets? storedsecrets = scc?.SecretStoreKey is not null
             ? await sqlConnAuthPwdStore.RetrieveAsync(scc.SecretStoreKey) : null;
         if (storedsecrets is not null)
         {
            if (sqlAuthAppPaths.UseDBNameRouting && (!string.IsNullOrEmpty(dbname) || dbname != storedsecrets.DBName))
            {
               context.RejectPrincipal();
               context.HttpContext.Response.Redirect(
                   sqlAuthAppPaths.UriEscapedSqlPath(server, user)
                   + (string.IsNullOrWhiteSpace(dbname) ? "" : $"/{Uri.EscapeDataString(dbname)}"));
            }
            else
            {
               if (storedsecrets.RuleReValidationAfter.HasValue && storedsecrets.RuleReValidationAfter.Value < DateTime.UtcNow)
               {
                  SqlAuthRuleValidationResult validationresult = await ruleValidator
                      .ValidateAsync(new(server, user, storedsecrets.Password, storedsecrets.TrustServerCertificate));
                  if (validationresult.Exception is not null || validationresult.StoredSecrets is null)
                  {
                     context.RejectPrincipal();
                  }
                  else
                  {
                     await sqlConnAuthPwdStore.RenewAsync(scc!.SecretStoreKey, validationresult.StoredSecrets);
                     storedsecrets = validationresult.StoredSecrets;
                  }
               }


               context.HttpContext.Items[typeof(SqlAuthStoredSecrets)] = storedsecrets;
            }
         }
         else
         {
            context.RejectPrincipal();
         }
      }
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

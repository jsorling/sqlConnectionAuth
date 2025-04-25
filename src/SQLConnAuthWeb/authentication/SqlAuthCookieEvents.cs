using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Sorling.SqlConnAuthWeb.extenstions;

namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlAuthCookieEvents(ISqlAuthPwdStore sqlConnAuthPwdStore
   , ISqlAuthRuleValidator ruleValidator
   , SqlAuthAppPaths sqlAuthAppPaths) : CookieAuthenticationEvents
{
   public override Task SigningIn(CookieSigningInContext context) {
      string? server = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLSERVER)?.Value;
      string? user = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLUSERNAME)?.Value;

      context.CookieOptions.IsEssential = true;
      context.CookieOptions.Path
         = $"/{sqlAuthAppPaths.Root.Trim('/')}/{Uri.EscapeDataString(server!)}/{Uri.EscapeDataString(user!)}";
      context.CookieOptions.SameSite = SameSiteMode.Strict;

      return base.SigningIn(context);
   }

   public override async Task ValidatePrincipal(CookieValidatePrincipalContext context) {
      string? server = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLSERVER)?.Value;
      string? user = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLUSERNAME)?.Value;
      string? passwordref = context.Principal?.FindFirst(SqlAuthConsts.CLAIMSQLPASSWORDREF)?.Value;

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
         else
         {
            context.RejectPrincipal();
         }
      }
   }

   public override Task SigningOut(CookieSigningOutContext context) => base.SigningOut(context);

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

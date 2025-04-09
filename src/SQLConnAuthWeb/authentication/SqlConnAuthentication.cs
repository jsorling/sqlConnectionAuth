using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.extenstions;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlConnAuthentication : CookieAuthenticationEvents, ISqlConnAuthenticationService
{
   private readonly HttpContext _httpContext;
   private readonly ISqlConnAuthPwdStore _sqlConnAuthPwdStore;
   private SqlConnAuthStoredSecrets? _storedSecrets;
   private readonly string? _userName;
   private readonly string? _dbSrv;

   public SqlConnAuthentication(IHttpContextAccessor httpContextAccessor, ISqlConnAuthPwdStore sqlConnAuthPwdStore
      , IOptions<SqlConnAuthenticationOptions> options) {
      _httpContext = (httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor)))?.HttpContext
         ?? throw new InvalidOperationException("HttpContext is not accessible to authenticate");
      _sqlConnAuthPwdStore = sqlConnAuthPwdStore ?? throw new ArgumentNullException(nameof(sqlConnAuthPwdStore));
      Options = options?.Value ?? throw new ArgumentNullException(nameof(options));

      _dbSrv = _httpContext.Request.RouteValues[SqlConnAuthConsts.URLROUTEPARAMSRV] as string;
      _userName = _httpContext.Request.RouteValues[SqlConnAuthConsts.URLROUTEPARAMUSR] as string;
   }

   public string Version => BuildInfo.Version;

   public SqlConnAuthenticationOptions Options { get; }

   public async Task<IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes>> GetDBsAsync()
      => await SqlConnectionHelper.GetDbsAsync(new(_dbSrv!, _userName!, _storedSecrets!));

   public async Task<SqlConnAuthenticationResult> AuthenticateAsync(string sqlServer, string userName
      , SqlConnAuthStoredSecrets storedSecrets) {
      if (string.IsNullOrEmpty(sqlServer))
         throw new ArgumentException($"'{nameof(sqlServer)}' cannot be null or empty.", nameof(sqlServer));
      if (string.IsNullOrEmpty(userName))
         throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
      if (string.IsNullOrEmpty(storedSecrets.Password))
         throw new ArgumentException($"'{nameof(storedSecrets.Password)}' cannot be null or empty.", nameof(storedSecrets.Password));

      if (storedSecrets.TrustServerCertificate && !Options.AllowTrustServerCertificate)
         return new SqlConnAuthenticationResult(false, new ApplicationException("Trust server certificate not allowed"), null);

      SqlConnAuthenticationData sca = new(sqlServer, userName, storedSecrets);
      if (sca.IsWinAuth && !Options.AllowWinauth)
         return new SqlConnAuthenticationResult(false, new ApplicationException("Windows authentication not allowed"), null);

      SqlConnAuthenticationResult result = await SqlConnectionHelper.TryConnectWithResultAsync(sca);

      if (result.Success) {
         await SignoutAsync();

         List<Claim>? claims = [
            new Claim(SqlConnAuthConsts.CLAIMSQLSERVER, sqlServer, ClaimValueTypes.String
               , SqlConnAuthConsts.SQLCONNAUTHSCHEME),
            new Claim(SqlConnAuthConsts.CLAIMSQLUSERNAME, userName, ClaimValueTypes.String
               , SqlConnAuthConsts.SQLCONNAUTHSCHEME),
            new Claim(ClaimTypes.Name, $"{sqlServer}/{userName}", ClaimValueTypes.String
               , SqlConnAuthConsts.SQLCONNAUTHSCHEME),
            new Claim(SqlConnAuthConsts.CLAIMSQLPASSWORDREF, await _sqlConnAuthPwdStore.StoreAsync(storedSecrets)
               , ClaimValueTypes.String, SqlConnAuthConsts.SQLCONNAUTHSCHEME)
         ];

         ClaimsIdentity claimsidentity = new(claims, SqlConnAuthConsts.SQLCONNAUTHSCHEME);
         await _httpContext.SignInAsync(SqlConnAuthConsts.SQLCONNAUTHSCHEME, new ClaimsPrincipal(claimsidentity));
      }

      return result;
   }

   public SqlConnAuthenticationData SqlConnAuthenticationData => new(_dbSrv, _userName, _storedSecrets!);

   public object RouteValues => new { sqlauthparamsrv = _dbSrv, sqlauthparamusr = _userName };

   public string UriEscapedPath(string? server = null, string? user = null) 
      => $"/{Uri.EscapeDataString(Options.SqlRootPath.Trim('/'))}/{Uri.EscapeDataString(server ?? _dbSrv ?? "")}/{Uri.EscapeDataString(user ?? _userName ?? "")}/srv";

   public string SqlConnectionString(string? db) => SqlConnAuthenticationData.ConnectionString(db);

   public async Task SignoutAsync() => await _httpContext.SignOutAsync(SqlConnAuthConsts.SQLCONNAUTHSCHEME);

   public async override Task ValidatePrincipal(CookieValidatePrincipalContext context) {
      SqlConnAuthCookieClaims? scc = (context ?? throw new ArgumentNullException(nameof(context))).Principal?.Identities.SqlConnAuthCookieClaims();
      _storedSecrets = scc?.SecretStoreKey is not null ? await _sqlConnAuthPwdStore.RetrieveAsync(scc.SecretStoreKey) : null;

      if (_storedSecrets is not null && scc is not null) {
         SqlConnAuthenticationData sca = new(scc.Server, scc.UserName, _storedSecrets);
         if (_storedSecrets.Password is null || (sca.IsWinAuth && !Options.AllowWinauth)) {
            context.RejectPrincipal();
         }
      }
      else {
         context.RejectPrincipal();
      }
   }

   public override Task SigningIn(CookieSigningInContext context) {
      context.CookieOptions.Path
         = $"/{Options.SqlRootPath.Trim('/')}/{Uri.EscapeDataString(_dbSrv!)}/{Uri.EscapeDataString(_userName!)}";
      context.CookieOptions.SameSite = SameSiteMode.Strict;

      return base.SigningIn(context);
   }

   public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context) {
      Uri uri = new(context.RedirectUri);
      string? newredirecturl = uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port, UriFormat.UriEscaped)
         + $"{context.Options.LoginPath}/{Uri.EscapeDataString(_dbSrv!)}/{Uri.EscapeDataString(_userName!)}?"
         + uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);

      context.RedirectUri = newredirecturl;
      return base.RedirectToLogin(context);
   }
}

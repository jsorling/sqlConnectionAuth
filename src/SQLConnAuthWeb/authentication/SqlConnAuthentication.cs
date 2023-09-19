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
   private string? _password;
   private readonly string? _userName;
   private readonly string? _dbSrv;

   public SqlConnAuthentication(IHttpContextAccessor httpContextAccessor, ISqlConnAuthPwdStore sqlConnAuthPwdStore, IOptions<SqlConnAuthenticationOptions> options) {
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
      => await SqlConnectionHelper.GetDbsAsync(new(_dbSrv!, _userName!, _password!));

   public async Task<SqlConnAuthenticationResult> AuthenticateAsync(string sqlServer, string userName, string password) {
      if (string.IsNullOrEmpty(sqlServer))
         throw new ArgumentException($"'{nameof(sqlServer)}' cannot be null or empty.", nameof(sqlServer));
      if (string.IsNullOrEmpty(userName))
         throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
      if (string.IsNullOrEmpty(password))
         throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));

      SqlConnAuthenticationData sca = new(sqlServer, userName, password);
      if (sca.IsWinAuth && !Options.AllowWinauth)
         return new SqlConnAuthenticationResult(false, new ApplicationException("Windows authentication not allowed"), null);

      SqlConnAuthenticationResult result = await SqlConnectionHelper.TryConnectWithResultAsync(sca);

      if (result.Success) {
         await SignoutAsync();

         List<Claim>? claims = new() {
            new Claim(SqlConnAuthConsts.CLAIMSQLSERVER, sqlServer, ClaimValueTypes.String
               , SqlConnAuthConsts.SQLCONNAUTHSCHEME),
            new Claim(SqlConnAuthConsts.CLAIMSQLUSERNAME, userName, ClaimValueTypes.String
               , SqlConnAuthConsts.SQLCONNAUTHSCHEME),
            new Claim(ClaimTypes.Name, $"{sqlServer}/{userName}", ClaimValueTypes.String
               , SqlConnAuthConsts.SQLCONNAUTHSCHEME),
            new Claim(SqlConnAuthConsts.CLAIMSQLPASSWORDREF, await _sqlConnAuthPwdStore.StoreAsync(password)
               , ClaimValueTypes.String, SqlConnAuthConsts.SQLCONNAUTHSCHEME)
         };

         ClaimsIdentity claimsidentity = new(claims, SqlConnAuthConsts.SQLCONNAUTHSCHEME);
         await _httpContext.SignInAsync(SqlConnAuthConsts.SQLCONNAUTHSCHEME, new ClaimsPrincipal(claimsidentity));
      }

      return result;
   }

   public SqlConnAuthenticationData SqlConnAuthenticationData => new(_dbSrv, _userName, _password);

   public object RouteValues => new { sqlauthparamsrv = _dbSrv, sqlauthparamusr = _userName };

   public string UriEscapedPath => $"/{Uri.EscapeDataString(Options.SqlRootPath.Trim('/'))}/{Uri.EscapeDataString(_dbSrv ?? "")}/{Uri.EscapeDataString(_userName ?? "")}/srv";

   public string SqlConnectionString(string? db) => SqlConnAuthenticationData.ConnectionString(db);

   public async Task SignoutAsync() => await _httpContext.SignOutAsync(SqlConnAuthConsts.SQLCONNAUTHSCHEME);

   public async override Task ValidatePrincipal(CookieValidatePrincipalContext context) {
      SqlConnAuthenticationData? sca = (context ?? throw new ArgumentNullException(nameof(context))).Principal?.Identities.SQLConnAuthenticationData();
      if (sca is not null) {
         _password = await _sqlConnAuthPwdStore.RetrieveAsync(sca.Password!);
         if (_password is null || (sca.IsWinAuth && !Options.AllowWinauth)) {
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

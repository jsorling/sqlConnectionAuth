using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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
    public string SQLServer { get; } = httpContextAccessor.HttpContext?.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMSRV] as string
        ?? throw new ApplicationException("SQL server cannot be null or empty on route.");

    /// <inheritdoc/>
    public string UserName { get; } = httpContextAccessor.HttpContext?.Request.RouteValues[SqlAuthConsts.URLROUTEPARAMUSR] as string
        ?? throw new ApplicationException("SQL username cannot be null or empty on route.");

    /// <inheritdoc/>
    public string UriEscapedPath => _sqlAuthAppPaths.UriEscapedSqlPath(SQLServer, UserName);

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
    public async Task SignoutAsync()
        => await _httpContext.SignOutAsync(SqlAuthConsts.SQLAUTHSCHEME);

    /// <inheritdoc/>
    public string? GetConnectionString(string? database = null) {
        SqlAuthStoredSecrets? storedsecrets = _httpContext.Items[typeof(SqlAuthStoredSecrets)] as SqlAuthStoredSecrets;
        if (storedsecrets is not null)
        {
            SqlAuthConnectionstringProvider sca = new(SQLServer, UserName, storedsecrets);
            return sca.ConnectionString(database);
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SqlConnectionHelper.ListDBCmd.ListDBRes>> GetDBsAsync() {
        SqlAuthStoredSecrets? storedsecrets = _httpContext.Items[typeof(SqlAuthStoredSecrets)] as SqlAuthStoredSecrets;
        if (storedsecrets is not null)
        {
            SqlAuthConnectionstringProvider sca = new(SQLServer, UserName, storedsecrets);
            return await SqlConnectionHelper.GetDbsAsync(sca);
        }

        return [];
    }
}

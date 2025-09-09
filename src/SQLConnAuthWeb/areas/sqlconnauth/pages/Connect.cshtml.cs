using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// Page model for the Connect page. Handles SQL authentication via password or Windows Authentication.
/// </summary>
/// <remarks>
/// This page allows users to authenticate to a SQL Server using either SQL authentication (username/password)
/// or Windows Authentication, depending on configuration and route context.
/// </remarks>
/// <param name="sqlConnAuthService">The SQL authentication service used for authentication operations.</param>
/// <param name="sqlAuthPwdStore">The password store for temporary and persistent credentials.</param>
/// <param name="options">The options monitor for SQL authentication configuration.</param>
/// <param name="sqlAuthContext">The authentication context providing connection and user details.</param>
[AllowAnonymous]
[RequireHttps]
public class ConnectModel(
    ISqlAuthService sqlConnAuthService,
    ISqlAuthPwdStore sqlAuthPwdStore,
    IOptionsMonitor<SqlAuthOptions> options,
    ISqlAuthContext sqlAuthContext
) : PageModel
{
    /// <summary>
    /// Gets or sets the input model for password and trust server certificate options.
    /// </summary>
    [BindProperty]
    public InputPasswordModel Input { get; set; } = new();

    private readonly SqlAuthOptions _sqlAuthOptions = options.CurrentValue;

    /// <summary>
    /// Gets a value indicating whether Windows Authentication is enabled and allowed for the current context.
    /// </summary>
    public bool IsWinAuth { get; private set; }

    /// <summary>
    /// Gets the database route parameter from the current request, if present.
    /// </summary>
    public string? RouteParamDb => Request.RouteValues[SqlAuthConsts.URLROUTEPARAMDB]?.ToString();

    /// <summary>
    /// Gets the SQL Server name from the authentication context.
    /// </summary>
    public string SQLServer => sqlAuthContext.SqlServer;

    /// <summary>
    /// Gets the user name from the authentication context.
    /// </summary>
    public string SqlUserName => sqlAuthContext.SqlUserName;

    /// <summary>
    /// Handles GET requests to the Connect page. Sets the <see cref="IsWinAuth"/> property based on the user and configuration.
    /// </summary>
    public void OnGet()
        => IsWinAuth = SqlUserName == SqlAuthConsts.WINDOWSAUTHENTICATION
            && _sqlAuthOptions.AllowIntegratedSecurity;

    /// <summary>
    /// Handles POST requests to the Connect page. Performs authentication and redirects or returns the page as appropriate.
    /// </summary>
    /// <param name="sqlauthparamdb">The database name from the route parameter, if using DB name routing.</param>
    /// <param name="returnUrl">The URL to redirect to on successful authentication, or null to use the default.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the result of the operation: a redirect on success, or the page with errors on failure.
    /// </returns>
    public async Task<IActionResult> OnPostAsync(
        [FromRoute] string? sqlauthparamdb,
        [FromQuery] string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            if (sqlAuthContext.AppPaths.UseDBNameRouting)
            {
                // Test authentication with temporary password and DB name routing
                SqlAuthenticationResult testauthenticateresult =
                    await sqlConnAuthService.TestAuthenticateAsync(Input, sqlauthparamdb);

                if (!testauthenticateresult.Success && testauthenticateresult.Exception is not null)
                {
                    ModelState.AddModelError("Password", testauthenticateresult.Exception.Message);
                    return Page();
                }
                else
                {
                    // Store temporary password and redirect to selectdb page
                    string tmppwdkey = await sqlAuthPwdStore.SetTempPasswordAsync(
                        SqlUserName, SQLServer, Input.Password, Input.TrustServerCertificate);

                    RouteValueDictionary routevalues = new()
                    {
                        { "area", SqlAuthConsts.SQLAUTHAREA },
                        { SqlAuthConsts.URLROUTEPARAMSRV, SQLServer },
                        { SqlAuthConsts.URLROUTEPARAMUSR, SqlUserName },
                        { SqlAuthConsts.URLROUTEPARAMTEMPPWD, tmppwdkey }
                    };
                    if (returnUrl != null)
                        routevalues["returnUrl"] = returnUrl;

                    return RedirectToPage("selectdb", routevalues);
                }
            }
            else
            {
                // Authenticate using standard SQL authentication
                SqlAuthenticationResult result = await sqlConnAuthService.AuthenticateAsync(Input);

                if (!result.Success && result.Exception is not null)
                {
                    ModelState.AddModelError("Password", result.Exception.Message);
                }

                return result.Success
                    ? Redirect(returnUrl ?? sqlAuthContext.AppPaths.UriEscapedSqlPath(sqlAuthContext.SqlServer, sqlAuthContext.SqlUserName))
                    : Page();
            }
        }

        // Model state is invalid or authentication failed
        return Page();
    }
}

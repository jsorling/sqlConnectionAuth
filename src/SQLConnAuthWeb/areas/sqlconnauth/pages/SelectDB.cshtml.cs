using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// PageModel for selecting a database after SQL authentication.
/// Handles GET and POST requests for database selection in the SQL authentication workflow.
/// </summary>
/// <remarks>
/// This page allows users to select a database after authenticating with a temporary password.
/// </remarks>
/// <param name="sqlConnAuthenticationService">The SQL authentication service for validating credentials and authenticating users.</param>
/// <param name="sqlAuthDBAccess">The SQL database access service for retrieving available databases.</param>
/// <param name="pwdStore">The password store service for managing temporary passwords.</param>
/// <param name="sqlAuthContext">The authentication context providing connection and user details.</param>
[AllowAnonymous]
[RequireHttps]
public class SelectDBModel(
    ISqlAuthService sqlConnAuthenticationService,
    ISqlAuthDBAccess sqlAuthDBAccess,
    ISqlAuthPwdStore pwdStore,
    ISqlAuthContext sqlAuthContext
) : PageModel
{
    /// <summary>
    /// Gets or sets the input model for database selection.
    /// </summary>
    [BindProperty]
    public InputSelectDBModel Input { get; set; } = new();

    /// <summary>
    /// Validates the temporary password asynchronously by testing authentication.
    /// </summary>
    /// <param name="tmpPwd">The temporary password info to validate.</param>
    /// <returns>A <see cref="Task{SqlAuthenticationResult}"/> representing the authentication result.</returns>
    private async Task<SqlAuthenticationResult> ValidateTmpPwdAsync(SqlAuthTempPasswordInfo tmpPwd)
        => await sqlConnAuthenticationService.TestAuthenticateAsync(tmpPwd, null);

    /// <summary>
    /// Retrieves the temporary password info from the request route values or context.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{SqlAuthTempPasswordInfo}"/> representing the asynchronous operation. The result contains the temporary password info, or null if not found.
    /// </returns>
    private async Task<SqlAuthTempPasswordInfo?> GetTempPasswordInfoAsync()
    {
        string? tmppwdkey = sqlAuthContext.TempPwdKey;
        return string.IsNullOrEmpty(tmppwdkey) ? null : await pwdStore.PeekTempPasswordAsync(tmppwdkey);
    }

    /// <summary>
    /// Retrieves the list of databases for the given temporary password.
    /// </summary>
    /// <param name="tmpPwd">The temporary password info used for database access.</param>
    /// <returns>
    /// A <see cref="Task{IEnumerable{ISqlDatabase}}"/> representing the asynchronous operation. The result contains a collection of SQL databases.
    /// </returns>
    private async Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthTempPasswordInfo tmpPwd)
        => await sqlAuthDBAccess.GetDatabasesAsync(new(sqlAuthContext.SqlServer, sqlAuthContext.SqlUserName, tmpPwd));

    /// <summary>
    /// Builds the route values for redirection, including query parameters from the current request.
    /// </summary>
    /// <returns>A <see cref="RouteValueDictionary"/> for redirection.</returns>
    private RouteValueDictionary GetRedirectRouteValues()
    {
        RouteValueDictionary tor = new()
        {
            { "area", SqlAuthConsts.SQLAUTHAREA },
            { SqlAuthConsts.URLROUTEPARAMSRV, sqlAuthContext.SqlServer },
            { SqlAuthConsts.URLROUTEPARAMUSR, sqlAuthContext.SqlUserName }
        };

        foreach (KeyValuePair<string, StringValues> q in Request.Query)
        {
            tor.Add(q.Key, q.Value);
        }

        return tor;
    }

    /// <summary>
    /// Replaces the database placeholder in the return URL with the selected database name.
    /// </summary>
    /// <param name="returnUrl">The return URL containing the placeholder.</param>
    /// <param name="db">The selected database name to insert.</param>
    /// <returns>The updated return URL, or null if <paramref name="returnUrl"/> is null.</returns>
    private static string? GetReturnURLSelectedDB(string? returnUrl, string db)
        => returnUrl?.Replace(SqlAuthConsts.RETURNURLSELECTDBPLACEHOLDER, db);

    /// <summary>
    /// Gets the application path URL for the selected database, constructing the route as needed.
    /// </summary>
    /// <param name="db">The selected database name, or null for the default.</param>
    /// <returns>The application path URL as a string.</returns>
    /// <exception cref="NullReferenceException">Thrown if the URL cannot be created.</exception>
    private string GetAppPathUrl(string? db = null)
    {
        RouteValueDictionary routevalues = new()
        {
            { "area", null },
            { SqlAuthConsts.URLROUTEPARAMSRV, sqlAuthContext.SqlServer },
            { SqlAuthConsts.URLROUTEPARAMUSR, sqlAuthContext.SqlUserName }
        };

        if (sqlAuthContext.AppPaths.UseDBNameRouting && db is not null)
        {
            routevalues.Add(SqlAuthConsts.URLROUTEPARAMDB, db);
        }

        string page = $"{sqlAuthContext.AppPaths.Root}{(sqlAuthContext.AppPaths.Root == "/" ? "" : "/")}index";

        return (Url.Page(page, routevalues)
            ?? throw new NullReferenceException($"Failed to create return URL in select database for page {page}")) + "/";
    }

    /// <summary>
    /// Handles GET requests to the page. Validates authentication and populates the database list for selection.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> for the page or a redirect to the connect page if authentication fails.
    /// </returns>
    public async Task<IActionResult> OnGetAsync()
    {
        SqlAuthTempPasswordInfo? tmppwd = await GetTempPasswordInfoAsync();
        if (tmppwd is null)
        {
            return RedirectToPage("connect", GetRedirectRouteValues());
        }

        SqlAuthenticationResult tresult = await ValidateTmpPwdAsync(tmppwd);

        if (!tresult.Success)
        {
            ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
            return Page();
        }
        else
        {
            Input.Databases = await GetDatabasesAsync(tmppwd);
            return Page();
        }
    }

    /// <summary>
    /// Handles POST requests to the page. Validates the selected database and authenticates the user for that database.
    /// </summary>
    /// <param name="returnUrl">The return URL to redirect to after successful authentication, or null to use the default.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> for the page or a redirect to the selected database or connect page if authentication fails.
    /// </returns>
    public async Task<IActionResult> OnPostAsync([FromQuery] string? returnUrl = null)
    {
        SqlAuthTempPasswordInfo? tmppwd = await GetTempPasswordInfoAsync();
        if (tmppwd is null)
        {
            return RedirectToPage("connect", GetRedirectRouteValues());
        }

        SqlAuthenticationResult tresult = await ValidateTmpPwdAsync(tmppwd);

        if (!tresult.Success)
        {
            ModelState.AddModelError(string.Empty, tresult.Exception?.Message ?? "Unknown error occurred during authentication.");
            return Page();
        }
        else if (!ModelState.IsValid)
        {
            return Page();
        }
        else
        {
            // Check if the selected database exists in the list
            IEnumerable<ISqlDatabase> dbs = await GetDatabasesAsync(tmppwd);
            if (dbs.Count(w => w.Name == Input.DBName) != 1)
            {
                ModelState.AddModelError(string.Empty, $"Database {Input.DBName} not found");
                Input.Databases = dbs;
                return Page();
            }

            SQLAuthenticateRequest authenticaterequest = new(tmppwd.Password, Input.DBName, tmppwd.TrustServerCertificate);
            SqlAuthenticationResult authresult = await sqlConnAuthenticationService.AuthenticateAsync(authenticaterequest);

            if (!authresult.Success && authresult.Exception is not null)
            {
                ModelState.AddModelError(string.Empty, authresult.Exception.Message);
            }

            if (authresult.Success)
            {
                // Remove temporary password after successful authentication
                await pwdStore.RemoveAsync(sqlAuthContext.TempPwdKey ?? string.Empty);
                string dbreturn = GetReturnURLSelectedDB(returnUrl, Input.DBName)
                    ?? GetAppPathUrl(Input.DBName);
                return Redirect(dbreturn);
            }

            return Page();
        }
    }
}

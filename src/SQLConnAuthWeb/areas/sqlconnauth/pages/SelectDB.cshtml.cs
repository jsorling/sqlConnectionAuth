using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor.models;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

/// <summary>
/// PageModel for selecting a database after SQL authentication.
/// Handles GET and POST requests for database selection.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SelectDBModel"/> class.
/// </remarks>
/// <param name="sqlConnAuthenticationService">The SQL authentication service.</param>
/// <param name="sqlAuthDBAccess">The SQL database access service.</param>
/// <param name="pwdStore">The password store service.</param>
/// <param name="sqlAuthAppPaths">The application path configuration.</param>
[AllowAnonymous]
[RequireHttps]
public class SelectDBModel(ISqlAuthService sqlConnAuthenticationService
      , ISqlAuthDBAccess sqlAuthDBAccess
      , ISqlAuthPwdStore pwdStore
      , SqlAuthAppPaths sqlAuthAppPaths
      ) : PageModel
{
   /// <summary>
   /// Gets or sets the input model for database selection.
   /// </summary>
   [BindProperty]
   public InputSelectDBModel Input { get; set; } = new();

   /// <summary>
   /// Validates the temporary password asynchronously.
   /// </summary>
   /// <param name="tmpPwd">The temporary password info.</param>
   /// <returns>The authentication result.</returns>
   private async Task<SqlAuthenticationResult> ValidateTmpPwdAsync(SqlAuthTempPasswordInfo tmpPwd)
      => await sqlConnAuthenticationService.TestAuthenticateAsync(tmpPwd, null);

   /// <summary>
   /// Retrieves the temporary password info from the request route values.
   /// </summary>
   /// <returns>The temporary password info, or null if not found.</returns>
   private async Task<SqlAuthTempPasswordInfo?> GetTempPasswordInfoAsync() {
      string? tmppwdkey = Request.RouteValues[SqlAuthConsts.URLROUTEPARAMTEMPPWD] as string;
      return string.IsNullOrEmpty(tmppwdkey) ? null : await pwdStore.PeekTempPasswordAsync(tmppwdkey);
   }

   /// <summary>
   /// Retrieves the list of databases for the given temporary password.
   /// </summary>
   /// <param name="tmpPwd">The temporary password info.</param>
   /// <returns>A collection of SQL databases.</returns>
   private async Task<IEnumerable<ISqlDatabase>> GetDatabasesAsync(SqlAuthTempPasswordInfo tmpPwd)
      => await sqlAuthDBAccess.GetDatabasesAsync(new(HttpContext.GetSqlAuthServer(), HttpContext.GetSqlAuthUserName(), tmpPwd));

   /// <summary>
   /// Builds the route values for redirection, including query parameters.
   /// </summary>
   /// <returns>The route value dictionary for redirection.</returns>
   private RouteValueDictionary GetRedirectRouteValues() {
      RouteValueDictionary tor = new()
      {
         { "area", SqlAuthConsts.SQLAUTHAREA },
         { SqlAuthConsts.URLROUTEPARAMSRV, HttpContext.GetSqlAuthServer() },
         { SqlAuthConsts.URLROUTEPARAMUSR, HttpContext.GetSqlAuthUserName() }
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
   /// <param name="db">The selected database name.</param>
   /// <returns>The updated return URL, or null if input is null.</returns>
   private static string? GetReturnURLSelectedDB(string? returnUrl, string db)
      => returnUrl?.Replace(SqlAuthConsts.RETURNURLSELECTDBPLACEHOLDER, db);

   /// <summary>
   /// Gets the application path URL for the selected database.
   /// </summary>
   /// <param name="db">The selected database name.</param>
   /// <returns>The application path URL.</returns>
   /// <exception cref="NullReferenceException">Thrown if the URL cannot be created.</exception>
   private string GetAppPathUrl(string? db = null) {
      RouteValueDictionary routevalues = new() {
         { "area", null },
         { SqlAuthConsts.URLROUTEPARAMSRV, HttpContext.GetSqlAuthServer() },
         { SqlAuthConsts.URLROUTEPARAMUSR, HttpContext.GetSqlAuthUserName() }
      };

      if (sqlAuthAppPaths.UseDBNameRouting && db is not null)
      {
         routevalues.Add(SqlAuthConsts.URLROUTEPARAMDB, db);
      }

      string page = $"{sqlAuthAppPaths.Root}{(sqlAuthAppPaths.Root=="/" ? "" : "/")}index";

      return Url.Page(page, routevalues)
         ?? throw new NullReferenceException($"Failed to create return URL in select database for page {page}");
   }

   /// <summary>
   /// Handles GET requests to the page. Validates authentication and populates the database list.
   /// </summary>
   /// <returns>An <see cref="IActionResult"/> for the page or a redirect.</returns>
   public async Task<IActionResult> OnGetAsync() {
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
         //List<ISqlDatabase> addnotex = [.. Input.Databases];
         //addnotex.Add(new SqlDatabase("not_exist"));

         //Random random = new();
         //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
         //for (int i = 10; i < 300; i++)
         //{
         //   int length = random.Next(6, 31); // random length between 6 and 30 (inclusive)
         //   string result = new([.. Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)])]);
         //   addnotex.Add(new SqlDatabase($"P{i}abc{result}"));
         //}

         //Input.Databases = addnotex;
         return Page();
      }
   }

   /// <summary>
   /// Handles POST requests to the page. Validates the selected database and authenticates.
   /// </summary>
   /// <param name="returnUrl">The return URL to redirect to after successful authentication.</param>
   /// <returns>An <see cref="IActionResult"/> for the page or a redirect.</returns>
   public async Task<IActionResult> OnPostAsync([FromQuery] string? returnUrl = null) {
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
         //check db-filter
         IEnumerable<ISqlDatabase> dbs = await GetDatabasesAsync(tmppwd);
         //Input.Databases = dbs;
         if (dbs.Count(w => w.Name == Input.DBName) != 1)
         {
            ModelState.AddModelError(string.Empty, $"Database {Input.DBName} not found");
            Input.Databases = dbs;
            return Page();
         }

         SQLAuthenticateRequest authenticaterequest = new(tmppwd.Password, tmppwd.TrustServerCertificate);
         SqlAuthenticationResult authresult = await sqlConnAuthenticationService.AuthenticateAsync(authenticaterequest);

         if (!authresult.Success && authresult.Exception is not null)
         {
            ModelState.AddModelError(string.Empty, authresult.Exception.Message);
         }

         string dbreturn = GetReturnURLSelectedDB(returnUrl, Input.DBName)
            ?? GetAppPathUrl(Input.DBName);
         return authresult.Success ? Redirect(dbreturn)
            : Page();
      }
   }
}

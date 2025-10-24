# Sorling.SqlConnAuthWeb

Sorling.SqlConnAuthWeb is a library for ASP.NET Core Razor Pages that provides a SQL Server connection manager tailored for administrative and management applications targeting SQL Server. This library enables you to build secure interfaces where users authenticate and manage SQL Server connections directly, making it ideal for tools and dashboards used by database administrators or power users.

## What can Sorling.SqlConnAuthWeb do?
- Authenticate users based on SQL Server connection credentials, supporting both SQL authentication and integrated security.
- Enforce authorization policies using SQL authentication context.
- Configure network access rules, such as allowing loopback or private network connections.
- Support trusted server certificates for secure connections.
- Integrate seamlessly with Razor Pages routing and authorization conventions.
- Provide user experience features like a theme switcher stored in local storage.

## UI
Sorling.SqlConnAuthWeb comes with a complete, ready-to-use UI for the authentication process, including login and connection management pages. If you need to customize the look, feel, or behavior, you can override the default UI by replacing the Razor Pages in the provided area with your own implementations. This follows the standard ASP.NET Core Razor Pages area view replacement pattern, allowing full control over the authentication experience.

**Connection Error Handling and Security Considerations**

Connection errors encountered during the authentication process are propagated to the UI. This means that details about failed connections, such as error messages from SQL Server or the application, may be displayed to users. While this can be helpful for troubleshooting in development or for administrative tools, it can also inadvertently disclose sensitive information about your application or SQL Server installation (such as server names, database structure, or authentication modes). For public-facing or production applications, you should carefully assess the level of detail exposed in error messages and consider customizing error handling to avoid leaking information that could aid an attacker. For management or admin-only applications, this behavior may be desirable, but always review your application's threat model and compliance requirements.

## Integrated Security
Integrated Security (Windows Authentication) allows users to authenticate to SQL Server using their Windows credentials. This feature works only in Windows environments and is not supported when running the application on Linux. When using Integrated Security, if the web server is not running in Windows authentication mode (such as with HTTPSys or IIS configured for Windows authentication), the web server's own Windows credentials will be used for SQL Server connections, not the end user's credentials. This means that unless your server is configured to forward the user's Windows identity, all SQL connections will be made as the web server process identity, which may not provide the desired security or auditing behavior. Always review your hosting and authentication configuration to ensure it matches your application's requirements.

## Password Handling
By default, user passwords are stored only in memory for the duration of the session, ensuring they are not persisted to disk or external storage. For advanced scenarios or stricter security requirements, you can implement your own custom password storage mechanism by extending the library's interfaces.

## IP Allow List, Private Network, and Loopback Controls

Sorling.SqlConnAuthWeb provides flexible network access control for SQL authentication. You can restrict which SQL Server IP addresses or ranges can be authenticated against by configuring the `AllowedIPAddresses` property. This property accepts single IPs, CIDR ranges, or subnet mask notation. If the allow-list is set (not empty), only authentication requests targeting SQL Servers with matching IPs/ranges are permitted. If the allow-list is empty, the following options apply:

- **AllowLoopbackConnections**: Controls whether authentication requests against loopback SQL Server addresses (e.g., `127.0.0.1`, `::1`) are permitted.
- **AllowPrivateNetworkConnections**: Controls whether authentication requests against private network SQL Server addresses (e.g., `192.168.x.x`, `10.x.x.x`, etc.) are permitted.

If both options are set to `true` and the allow-list is empty, authentication requests against all network locations are allowed. If either is set to `false`, requests to those address types are blocked unless explicitly allowed in the allow-list.

The allow-list takes precedence: if it is not empty, only those SQL Server IPs/ranges can be authenticated against, regardless of the other settings.

**Example usage:**
- To allow authentication only against specific SQL Server IPs or ranges, populate `AllowedIPAddresses`.
- To allow authentication against all private and loopback SQL Server addresses, leave the allow-list empty and set both options to `true`.
- To block authentication against private or loopback SQL Server addresses, set the corresponding option to `false`.

## Routing

The routing system in Sorling.SqlConnAuthWeb is designed to support SQL authentication scenarios by dynamically generating URL patterns that include SQL Server connection parameters. This enables each authentication session to be uniquely identified by the server and user being authenticated against, and allows for flexible, parameterized navigation within the Razor Pages application.

### How Routing Works

Routing is configured in `Program.cs` using the `SqlAuthAppPaths` object, which defines the root and tail segments for SQL authentication URLs. For example:

```csharp
SqlAuthAppPaths sqlauthpath = new("/db", "srv");
```

This configuration, combined with the custom route conventions, results in URLs like:

```
https://localhost:7061/db/192.168.1.233/sa/srv
```

- `/db` is the root path for SQL authentication pages.
- `192.168.1.233` is the SQL Server address (injected as a route parameter).
- `sa` is the SQL Server username (also a route parameter).
- `/srv` is an optional tail segment, as defined in `SqlAuthAppPaths`.

#### UseDBNameRouting

If the `UseDBNameRouting` property is set to `true` in your `SqlAuthAppPaths` configuration, the route pattern is extended to include the database name as an additional route parameter. This allows URLs to uniquely identify not only the server and user, but also the target database for the authentication session. For example:

```
https://localhost:7061/db/192.168.1.233/sa/mydatabase/srv
```

- `mydatabase` is the SQL Server database name (added as a route parameter when `UseDBNameRouting` is enabled).

When `UseDBNameRouting` is enabled, the custom route convention injects the database name into the route template, and Razor Pages can access it directly from the route data. This is useful for scenarios where authentication or authorization depends on the specific database context, not just the server and user.

To enable this, set `UseDBNameRouting = true` in your `SqlAuthAppPaths` instance.

The route parameters are injected by the custom convention, so Razor Pages can access them directly from the route data.

### How SqlAuthAppPaths Works

`SqlAuthAppPaths` is a configuration object that defines the root and tail segments for all SQL authentication-related routes. The `Root` property sets the base path (e.g., `/db`), and the `Tail` property can add an additional segment (e.g., `/srv`). These values are used by the route convention to generate the full URL structure for authentication pages.

The `UseDBNameRouting` property on `SqlAuthAppPaths` controls whether the database name is included as a route parameter in authentication URLs.

### Custom Route Convention: AddSqlAuthRazorPageRouteConventions

The extension method `AddSqlAuthRazorPageRouteConventions` (see `MvcBuilderExtensions.cs`) adds a custom Razor Pages route convention via the `SqlAuthPageRouteModelConvention` class. This convention rewrites the route templates for authentication pages to include the server and user as route parameters, and optionally appends the tail segment. For example, a page that would normally be at `/db` becomes accessible at `/db/{server}/{user}/srv`.

**How it works:**
- The convention inspects each Razor Page route.
- If the route starts with the configured root path, it rewrites the template to include `/{server}/{user}/[tail]`.
- This enables URLs to carry the SQL Server and user context, which can be used for authentication and authorization logic within the page handlers.

### Link Generation in Razor: `SqlAuthAnchorRouteTagHelper`

Use the `SqlAuthAnchorRouteTagHelper` to generate links that automatically include the current SQL auth route parameters.

- Import the tag helper into Razor Pages (once):

```csharp
@addTagHelper *, Sorling.SqlConnAuthWeb
```

- Use normal Razor Pages anchors; the helper will preserve/merge SQL auth params when needed:

```html
<!-- Sibling page link from /db/import/index to /db/import/upload -->
<a asp-page="./upload">Upload</a>

<!-- Navigate up one level -->
<a asp-page="../index">Back to DB home</a>

<!-- With handler and additional route value -->
<a asp-page="./upload" asp-page-handler="post" asp-route-id="42">Upload item</a>
```

- Example output when ambient route is `/db/192.168.1.233/sa/mydatabase/import`:

```html
<a href="/db/192.168.1.233/sa/mydatabase/import/upload">Upload</a>
<a href="/db/192.168.1.233/sa/mydatabase/index">Back to DB home</a>
<a href="/db/192.168.1.233/sa/mydatabase/import/upload?id=42&amp;handler=post">Upload item</a>
```

Notes:
- The helper only acts when the current request has SQL auth route values (server/user[/db]).
- It runs after the built-in AnchorTagHelper; if a required value is missing, it merges the ambient SQL auth values and computes the href.
- You no longer need to manually pass `sqlauthparamsrv`, `sqlauthparamusr`, or `sqlauthparamdb` in markup.

### Tag Helper: `sql-auth-context-script`

The `sql-auth-context-script` tag helper emits a script tag that exposes the current SQL auth route context to JavaScript as `window.SqlAuthContext`.

- Register the tag helper (once) in your Razor project:

```csharp
@addTagHelper *, Sorling.SqlConnAuthWeb
```

- Use it in a layout or page:

```html
<sql-auth-context-script />
```

- Output example:

```html
<script>
 window.SqlAuthContext = { SqlServer: '192.168.1.233', SqlUserName: 'sa', SqlDBName: 'mydatabase' };
</script>
```

- Consume it in JavaScript:

```html
<script>
 const ctx = window.SqlAuthContext || {};
 if (ctx.SqlServer && ctx.SqlUserName) {
 // Example: build a URL including the current SQL route context
 const parts = [
 'db',
 encodeURIComponent(ctx.SqlServer),
 encodeURIComponent(ctx.SqlUserName)
 ];
 if (ctx.SqlDBName) parts.push(encodeURIComponent(ctx.SqlDBName));
 const url = '/' + parts.join('/') + '/import/upload';
 // navigate or fetch
 // location.href = url;
 }
</script>
```

Note: Only route values are exposed (server, user, optional DB name). No secrets or passwords are emitted. Review whether exposing the database name is appropriate for your app.

### Redirect to SelectDB with a temporary password key

Use `RedirectToSelectDBWithTempPwdKeyResult` to send the user to the SQL auth Select DB page with a short?lived key that references their current (already authenticated) secrets. This avoids putting passwords in the URL and lets the user pick a database.

- What it does:
 - Generates a temporary password key via your configured `ISqlAuthPwdStore` from the current `ISqlAuthContext.StoredSecrets`.
 - Redirects to the SelectDB page in the `sqlconnauth` area with route values: `{server}/{user}/{temppwd}`.
 - Optionally carries a `returnUrl` query string that can include the placeholder `--sqlauthselecteddb`; the placeholder will be replaced with the selected DB name when the user submits the SelectDB form.

- Example usage from a PageModel:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

public class SomePageModel : PageModel
{
 private readonly ISqlAuthContext _ctx;
 public SomePageModel(ISqlAuthContext ctx) => _ctx = ctx;

 public IActionResult OnPostReSelectDb()
 {
 // Optional: where to go after the user picks a DB. You can omit returnUrl to go to the default app path.
 string? returnUrl = null;
 return _ctx.GetRedirectToSelectDBActionResult(returnUrl);
 }
}
```

- Example URL generated:

```
/sqlconnauth/{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamtemppwd}/selectdb
```

- Using the returnUrl placeholder:

```csharp
using Sorling.SqlConnAuthWeb;
using Sorling.SqlConnAuthWeb.authentication;

// Build a returnUrl that injects the DB selected on the SelectDB page
var route = new Microsoft.AspNetCore.Routing.RouteValueDictionary
{
 ["area"] = null,
 [SqlAuthConsts.URLROUTEPARAMSRV] = _ctx.SqlServer,
 [SqlAuthConsts.URLROUTEPARAMUSR] = _ctx.SqlUserName,
 [SqlAuthConsts.URLROUTEPARAMDB] = SqlAuthConsts.RETURNURLSELECTDBPLACEHOLDER,
};
string? returnUrl = Url.Page($"{_ctx.AppPaths.Root}/index", route) + "/";
return _ctx.GetRedirectToSelectDBActionResult(returnUrl);
```

### Root Path Authorization: AuthorizeSqlAuthRootPath

The extension method `AuthorizeSqlAuthRootPath` (see `MvcBuilderExtensions.cs`) configures Razor Pages to require a specific authorization policy for all pages under the SQL authentication root path. It does this by calling `options.Conventions.AuthorizeFolder` with the root path and a custom policy name. This ensures that all authentication-related pages are protected by the appropriate authorization logic, and only accessible to users who have passed SQL authentication.

## Accessing the Authenticated Connection String in Razor Pages

The recommended way to access the authenticated SQL connection string and related context in Razor Pages is now by injecting the `ISqlAuthContext` interface. This provides direct access to the connection string, SQL Server, user name, and other authentication context from within your PageModels or views.

### Accessing the Connection String in a PageModel

You can access the connection string and related context by injecting `ISqlAuthContext` into your PageModel. For example:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

public class IndexModel : PageModel
{
 private readonly ISqlAuthContext _sqlAuthContext;

 public IndexModel(ISqlAuthContext sqlAuthContext)
 {
 _sqlAuthContext = sqlAuthContext;
 }

 public string? ConnectionString => _sqlAuthContext.GetConnectionString();
 public string? SqlServer => _sqlAuthContext.SqlServer;
 public string? SqlUser => _sqlAuthContext.SqlUserName;
}
```

### Accessing the Connection String in a Razor View

You can also inject `ISqlAuthContext` directly into your Razor view using the `@inject` directive:

```csharp
@using Sorling.SqlConnAuthWeb.authentication
@inject ISqlAuthContext SqlAuthContext

<p>Current connection string: @SqlAuthContext.GetConnectionString()</p>
<p>SQL Server: @SqlAuthContext.SqlServer</p>
<p>SQL User: @SqlAuthContext.SqlUserName</p>
```

### Additional Context

- `ISqlAuthContext.ConnectionString` returns the connection string for the current authenticated SQL context.
- You can access the current SQL Server and user via `ISqlAuthContext.Server` and `ISqlAuthContext.UserName`.
- `ISqlAuthContext` can be injected anywhere dependency injection is available, including PageModels and Razor views.

This approach provides a more direct and flexible way to access SQL authentication context, compared to the previous method using HttpContext extension methods.

## Getting Started

To quickly get up and running, use the following minimal example. This demonstrates how to configure and launch a Razor Pages app with SQL connection authentication using Sorling.SqlConnAuthWeb.

**Note:** You must have a Razor Pages directory called `/db` in your project. This is required because the example configures the SQL authentication area to use `/db` as the root path for authentication-related pages.

### Program.cs

```csharp
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor;

SqlAuthAppPaths sqlauthpath = new("/db", UseDBNameRouting: true);
WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SqlAuthUIOptions>(builder.Configuration.GetSection("SqlAuthUIOptions"));
builder.Services.AddSqlConnAuthentication(sqlauthpath);

builder.Services.AddSqlConnAuthorization()
 .AddRazorPages()
 .AddSqlAuthRazorPageRouteConventions()
 .AddSqlAuthFilters()
 .AuthorizeSqlAuthRootPath();

WebApplication app = builder.Build();
app.UseHttpsRedirection()
 .UseStaticFiles()
 .UseRouting()
 .UseAuthentication()
 .UseAuthorization();

app.MapRazorPages();

app.Run();
```

### appsettings.json

```json
{
 "Logging": {
 "LogLevel": {
 "Default": "Information",
 "Microsoft.AspNetCore": "Warning"
 }
 },
 "AllowedHosts": "*",
 "SqlAuthOptions": {
 "AllowTrustServerCertificate": true,
 "AllowLoopbackConnections": true,
 "AllowPrivateNetworkConnections": true,
 "AllowedIPAddresses": [
 "127.0.0.1/32",
 "192.168.1.0/24"
 ],
 "ExcludeDatabaseFilter": [
 "mAst*"
 ],
 "IncludeDatabaseFilter": [
 "*"
 ]
 },
 "SqlAuthUIOptions": {
 "ThemeSwitcherLocalStorageName": "sqlauththeme",
 "MaxRecentItems":20
 }
}
```

## How the Example Works

The code above shows how to integrate SQL connection-based authentication and authorization into a Razor Pages application using Sorling.SqlConnAuthWeb. The key features demonstrated are:

- SQL Connection Authentication (with trusted server certificates)
- Network access rules (loopback, private network)
- Theme switcher support
- SQL Connection Authorization
- Custom routing and root path authorization for SQL authentication

---


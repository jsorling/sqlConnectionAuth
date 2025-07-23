# Sorling.SqlConnectionAuth

Sorling.SqlConnectionAuth is a library for ASP.NET Core Razor Pages that enables authentication and authorization using SQL Server connections. It allows you to secure your web applications by leveraging SQL Server credentials and connection properties, providing a flexible alternative to traditional identity systems. The library can also serve as a starting point for building management-type applications for SQL Server, enabling you to quickly create secure admin or management interfaces.

## What can SqlConnectionAuth do?
- Authenticate users based on SQL Server connection credentials, supporting both SQL authentication and integrated security.
- Enforce authorization policies using SQL authentication context.
- Configure network access rules, such as allowing loopback or private network connections.
- Support trusted server certificates for secure connections.
- Integrate seamlessly with Razor Pages routing and authorization conventions.
- Provide user experience features like a theme switcher stored in local storage.

## UI
Sorling.SqlConnectionAuth comes with a complete, ready-to-use UI for the authentication process, including login and connection management pages. If you need to customize the look, feel, or behavior, you can override the default UI by replacing the Razor Pages in the provided area with your own implementations. This follows the standard ASP.NET Core Razor Pages area view replacement pattern, allowing full control over the authentication experience.

**Connection Error Handling and Security Considerations**

Connection errors encountered during the authentication process are propagated to the UI. This means that details about failed connections, such as error messages from SQL Server or the application, may be displayed to users. While this can be helpful for troubleshooting in development or for administrative tools, it can also inadvertently disclose sensitive information about your application or SQL Server installation (such as server names, database structure, or authentication modes). For public-facing or production applications, you should carefully assess the level of detail exposed in error messages and consider customizing error handling to avoid leaking information that could aid an attacker. For management or admin-only applications, this behavior may be desirable, but always review your application's threat model and compliance requirements.

## Integrated Security
Integrated Security (Windows Authentication) allows users to authenticate to SQL Server using their Windows credentials. This feature works only in Windows environments and is not supported when running the application on Linux. When using Integrated Security, if the web server is not running in Windows authentication mode (such as with HTTPSys or IIS configured for Windows authentication), the web server's own Windows credentials will be used for SQL Server connections, not the end user's credentials. This means that unless your server is configured to forward the user's Windows identity, all SQL connections will be made as the web server process identity, which may not provide the desired security or auditing behavior. Always review your hosting and authentication configuration to ensure it matches your application's requirements.

## Password Handling
By default, user passwords are stored only in memory for the duration of the session, ensuring they are not persisted to disk or external storage. For advanced scenarios or stricter security requirements, you can implement your own custom password storage mechanism by extending the library's interfaces.

## IP Allow List, Private Network, and Loopback Controls

Sorling.SqlConnectionAuth provides flexible network access control for SQL authentication. You can restrict which SQL Server IP addresses or ranges can be authenticated against by configuring the `AllowedIPAddresses` property. This property accepts single IPs, CIDR ranges, or subnet mask notation. If the allow-list is set (not empty), only authentication requests targeting SQL Servers with matching IPs/ranges are permitted. If the allow-list is empty, the following options apply:

- **AllowLoopbackConnections**: Controls whether authentication requests against loopback SQL Server addresses (e.g., `127.0.0.1`, `::1`) are permitted.
- **AllowPrivateNetworkConnections**: Controls whether authentication requests against private network SQL Server addresses (e.g., `192.168.x.x`, `10.x.x.x`, etc.) are permitted.

If both options are set to `true` and the allow-list is empty, authentication requests against all network locations are allowed. If either is set to `false`, requests to those address types are blocked unless explicitly allowed in the allow-list.

**Example usage:**
- To allow authentication only against specific SQL Server IPs or ranges, populate `AllowedIPAddresses`.
- To allow authentication against all private and loopback SQL Server addresses, leave the allow-list empty and set both options to `true`.
- To block authentication against private or loopback SQL Server addresses, set the corresponding option to `false`.

The allow-list takes precedence: if it is not empty, only those SQL Server IPs/ranges can be authenticated against, regardless of the other settings.

## Routing

The routing system in Sorling.SqlConnectionAuth is designed to support SQL authentication scenarios by dynamically generating URL patterns that include SQL Server connection parameters. This enables each authentication session to be uniquely identified by the server and user being authenticated against, and allows for flexible, parameterized navigation within the Razor Pages application.

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

The route parameters are injected by the custom convention, so Razor Pages can access them directly from the route data.

### How SqlAuthAppPaths Works

`SqlAuthAppPaths` is a configuration object that defines the root and tail segments for all SQL authentication-related routes. The `Root` property sets the base path (e.g., `/db`), and the `Tail` property can add an additional segment (e.g., `/srv`). These values are used by the route convention to generate the full URL structure for authentication pages.

### Custom Route Convention: AddSqlAuthRazorPageRouteConventions

The extension method `AddSqlAuthRazorPageRouteConventions` (see `MvcBuilderExtensions.cs`) adds a custom Razor Pages route convention via the `SqlAuthPageRouteModelConvention` class. This convention rewrites the route templates for authentication pages to include the server and user as route parameters, and optionally appends the tail segment. For example, a page that would normally be at `/db` becomes accessible at `/db/{server}/{user}/srv`.

**How it works:**
- The convention inspects each Razor Page route.
- If the route starts with the configured root path, it rewrites the template to include `/{server}/{user}/[tail]`.
- This enables URLs to carry the SQL Server and user context, which can be used for authentication and authorization logic within the page handlers.

### Root Path Authorization: AuthorizeSqlAuthRootPath

The extension method `AuthorizeSqlAuthRootPath` (see `MvcBuilderExtensions.cs`) configures Razor Pages to require a specific authorization policy for all pages under the SQL authentication root path. It does this by calling `options.Conventions.AuthorizeFolder` with the root path and a custom policy name. This ensures that all authentication-related pages are protected by the appropriate authorization logic, and only accessible to users who have passed SQL authentication.

## Accessing the Authenticated Connection String in Razor Pages

To access the authenticated SQL connection string within your Razor PageModels or views, you can inject the `ISqlAuthService` interface. This service provides methods to retrieve the current connection string, user, and server context for the authenticated session.

### Injecting ISqlAuthService in a PageModel

You can inject `ISqlAuthService` via constructor injection in your PageModel:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

public class IndexModel : PageModel
{
    private readonly ISqlAuthService _sqlAuthService;

    public IndexModel(ISqlAuthService sqlAuthService)
    {
        _sqlAuthService = sqlAuthService;
    }

    public string? ConnectionString => _sqlAuthService.GetConnectionString();
}
```

### Accessing the Connection String in a Razor View

You can also inject `ISqlAuthService` directly into a Razor view using the `@inject` directive:

```csharp
@inject Sorling.SqlConnAuthWeb.authentication.ISqlAuthService SqlAuthService

<p>Current connection string: @SqlAuthService.GetConnectionString()</p>
```

### Additional Context
- `ISqlAuthService.GetConnectionString()` returns the connection string for the current authenticated SQL context.
- You can also access the current SQL Server and user via `ISqlAuthService.SQLServer` and `ISqlAuthService.UserName`.
- The service is available for injection in both PageModels and Razor views, making it easy to use the authenticated connection throughout your application.

## Program.cs
```C#
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.extenstions;

SqlAuthAppPaths sqlauthpath = new("/db", "srv");
WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlConnAuthentication(sqlauthpath, o => {
   o.AllowIntegratedSecurity = true;
   o.ThemeSwitcherLocalStorageName = "theme";
   o.AllowTrustServerCertificate = true;
   o.AllowLoopbackConnections = true;
   o.AllowPrivateNetworkConnections = true;
})
   .AddSqlConnAuthorization()
   .AddRazorPages()
   .AddSqlAuthRazorPageRouteConventions(sqlauthpath)
   .AuthorizeSqlAuthRootPath(sqlauthpath);

WebApplication app = builder.Build();
app.UseHttpsRedirection()
   .UseStaticFiles()
   .UseRouting()
   .UseAuthentication()
   .UseAuthorization();

app.MapRazorPages();
app.Run();
```
### What does the sqlConnectionAuth code do?
The code in `Program.cs` demonstrates how to integrate SQL connection-based authentication and authorization into a Razor Pages application using the sqlConnectionAuth library. The key features added by sqlConnectionAuth are:

- **SQL Connection Authentication:**
  - Adds authentication based on SQL Server connections, supporting integrated security and trusted server certificates.
  - Allows configuration of network access rules (loopback, private network).
  - Supports a theme switcher stored in local storage.

- **SQL Connection Authorization:**
  - Enables authorization policies based on SQL authentication.

- **Custom Routing for SQL Auth:**
  - Applies custom route conventions and root path authorization specific to SQL authentication.

These features extend a standard Razor Pages app to support secure, SQL-authenticated access and custom authorization logic.


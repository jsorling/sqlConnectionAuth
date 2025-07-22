# sqlConnectionAuth
## WebApp

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

### Program.cs
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
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

//https://www.meziantou.net/list-all-routes-in-an-asp-net-core-application.htm
if (app.Environment.IsDevelopment())
{
   _ = app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources)
      => string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
}

app.Run();

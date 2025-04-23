using Sorling.SqlConnAuthWeb;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlConnAuthentication(o => {
   o.AllowIntegratedSecurity = true;
   o.ThemeSwitcherLocalStorageName = "theme";
   o.AllowTrustServerCertificate = true;
   //o.AllowLoopbackConnections = true;
   //o.AllowPrivateNetworkConnections = true;
})
   .AddSqlConnAuthorization()
   .AddRazorPages(options => {
      SqlAuthOptions? sqlconnauthoptions = builder.Configuration.Get<SqlAuthOptions>();
      if (sqlconnauthoptions != null)
      {
         // Authorize the SQL root path, comment out this line if you want to add your own authorization
         _ = options.Conventions.AuthorizeFolder(sqlconnauthoptions.SqlRootPath, SqlAuthConsts.SQLAUTHPOLICY);
         // Add route values to all Razor pages under the SQL root path, comment out this line if you want to add your own route values
         options.Conventions.Add(new SqlAuthPageRouteModelConvention(sqlconnauthoptions));
      }
   });

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

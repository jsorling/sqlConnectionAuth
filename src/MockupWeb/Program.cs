using Sorling.SqlConnAuthWeb;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlConnAuthentication(o => {
   o.AllowWinauth = true;
   o.ThemeSwitcherLocalStorageName = "theme";
   o.AllowTrustServerCertificate = true;
})
   .AddSqlConnAuthorization()
   .AddRazorPages(options => {
      SqlConnAuthenticationOptions? sqlconnauthoptions = builder.Configuration.Get<SqlConnAuthenticationOptions>();
      if (sqlconnauthoptions != null) {
         _ = options.Conventions.AuthorizeFolder(sqlconnauthoptions.SqlRootPath, SqlConnAuthConsts.SQLCONNAUTHPOLICY);
      }

      options.Conventions.Add(new SqlConnAuthPageRouteModelConvention());
   });

WebApplication app = builder.Build();
app.UseHttpsRedirection().UseStaticFiles().UseRouting()
   .UseAuthentication().UseAuthorization();

app.MapRazorPages();

//https://www.meziantou.net/list-all-routes-in-an-asp-net-core-application.htm
if (app.Environment.IsDevelopment()) {
   _ = app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources)
      => string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
}

app.Run();

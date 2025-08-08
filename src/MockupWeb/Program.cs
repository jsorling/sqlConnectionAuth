using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.extenstions;

SqlAuthAppPaths sqlauthpath = new("/db", UseDBNameRouting: false);
WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Bind SqlAuthOptions from configuration (appsettings.json) and allow override via delegate
builder.Services.AddSqlConnAuthentication(sqlauthpath, builder.Configuration.GetSection("SqlAuthOptions").Bind);

builder.Services.AddSqlConnAuthorization()
   .AddRazorPages()
   .AddSqlAuthRazorPageRouteConventions()
   .AuthorizeSqlAuthRootPath();

WebApplication app = builder.Build();
app.UseHttpsRedirection()
   .UseStaticFiles()
   .UseRouting()
   .UseAuthentication()
   .UseAuthorization();

app.MapRazorPages();

app.Run();

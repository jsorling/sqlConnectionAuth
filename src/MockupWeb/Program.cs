using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor;

SqlAuthAppPaths sqlauthpath = new("/db", UseDBNameRouting: false);
WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Bind SqlAuthOptions from configuration (appsettings.json) and allow override via delegate
builder.Services.AddSqlConnAuthentication(sqlauthpath, builder.Configuration.GetSection("SqlAuthOptions").Bind);

// Bind SqlAuthUIOptions from configuration for runtime updates
builder.Services.Configure<SqlAuthUIOptions>(builder.Configuration.GetSection("SqlAuthUIOptions"));

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

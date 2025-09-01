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
   .AuthorizeSqlAuthRootPath();

WebApplication app = builder.Build();
app.UseHttpsRedirection()
   .UseStaticFiles()
   .UseRouting()
   .UseAuthentication()
   .UseAuthorization();

app.MapRazorPages();

app.Run();

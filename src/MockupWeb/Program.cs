using System.Reflection;
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

Console.WriteLine($"Content root: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Appsettings path: {Path.Combine(builder.Environment.ContentRootPath, "appsettings.json")}");
Console.WriteLine($"Executing assembly path: {Assembly.GetExecutingAssembly().Location}");

WebApplication app = builder.Build();
app.UseHttpsRedirection()
   .UseStaticFiles()
   .UseRouting()
   .UseAuthentication()
   .UseAuthorization();

app.MapRazorPages();

app.Run();

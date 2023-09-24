# sqlConnectionAuth
## WebApp
### Program.cs
```C#
using Sorling.SqlConnAuthWeb;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlConnAuthentication(o=>{ o.AllowWinauth = true; 
   o.ThemeSwitcherLocalStorageName = "theme"; o.SqlRootPath = "sqlsrv"; })
   .AddSqlConnAuthorization().AddRazorPages(options => {
      _ = options.Conventions.AuthorizeFolder("/sqlsrv", SqlConnAuthConsts.SQLCONNAUTHPOLICY);
      options.Conventions.Add(new SqlConnAuthPageRouteModelConvention("sqlsrv"));
   });

WebApplication app = builder.Build();
app.UseHttpsRedirection().UseStaticFiles().UseRouting()
   .UseAuthentication().UseAuthorization();

app.MapRazorPages();

app.Run();
```
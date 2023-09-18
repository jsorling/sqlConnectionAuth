# sqlConnectionAuth
## WebApp
### Program.cs
```C#
using Sorling.SqlConnAuthWeb;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.extenstions;
using Sorling.SqlConnAuthWeb.razor;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddSQLConnAuthentication(o => { o.AllowWinauth = true; o.ThemeSwitcherLocalStorageName = "theme"; })
   .AddSQLConnAuthorization()
   .AddRazorPages(options => {
      _ = options.Conventions.AuthorizeFolder(builder.Configuration.Get<SqlConnAuthenticationOptions>().SqlPath, SqlConnAuthConsts.SQLCONNAUTHPOLICY);
      options.Conventions.Add(new SqlConnAuthPageRouteModelConvention(builder.Configuration.Get<SqlConnAuthenticationOptions>()));
   });

WebApplication app = builder.Build();
app.UseHttpsRedirection().UseStaticFiles().UseRouting()
   .UseAuthentication().UseAuthorization();

app.MapRazorPages();

app.Run();
```
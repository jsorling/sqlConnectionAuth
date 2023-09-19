using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.extenstions;
public static class ServiceCollectionExtenstions
{
   public static IServiceCollection AddSqlConnAuthentication(this IServiceCollection services, Action<SqlConnAuthenticationOptions> configureOptions) {
      _ = services.AddHttpContextAccessor()
         .AddScoped<ISqlConnAuthenticationService, SqlConnAuthentication>()
         .AddAuthentication()
         .AddCookie(SqlConnAuthConsts.SQLCONNAUTHSCHEME, options => {
            options.LoginPath = $"/{SqlConnAuthConsts.SQLCONNAUTHAREA}/connect";
            options.AccessDeniedPath = $"/{SqlConnAuthConsts.SQLCONNAUTHAREA}/accessdenied";
            options.LogoutPath = $"/{SqlConnAuthConsts.SQLCONNAUTHAREA}/disconnect";
            options.ExpireTimeSpan = new(30, 0, 0, 0, 0);
            options.SlidingExpiration = true;
            options.EventsType = typeof(ISqlConnAuthenticationService);
            options.Validate();
         });

      services.TryAddSingleton<ISqlConnAuthPwdStore, SqlConnAuthPwdMemoryStore>();
      _ = services.Configure(configureOptions);

      return services;
   }

   public static IServiceCollection AddSQLConnAuthentication(this IServiceCollection services)
      => services.AddSqlConnAuthentication(options => { });

   public static IServiceCollection AddSqlConnAuthorization(this IServiceCollection services)
      => services.AddAuthorization(option => option.AddPolicy(SqlConnAuthConsts.SQLCONNAUTHPOLICY,
         p => p.RequireAuthenticatedUser().AddAuthenticationSchemes(SqlConnAuthConsts.SQLCONNAUTHSCHEME)));
}


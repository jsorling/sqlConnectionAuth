using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.extenstions;
public static class ServiceCollectionExtenstions
{
   public static IServiceCollection AddSqlConnAuthentication(this IServiceCollection services
      , Action<SqlAuthOptions> configureOptions) {
      _ = services.AddHttpContextAccessor()
         .AddTransient<ISqlAuthService, SqlAuthService>()
         .AddAuthentication()
         .AddCookie(SqlAuthConsts.SQLAUTHSCHEME, options => {
            options.LoginPath = $"/{SqlAuthConsts.SQLAUTHAREA}/connect";
            options.AccessDeniedPath = $"/{SqlAuthConsts.SQLAUTHAREA}/accessdenied";
            options.LogoutPath = $"/{SqlAuthConsts.SQLAUTHAREA}/disconnect";
            options.ExpireTimeSpan = new(30, 0, 0, 0, 0);
            options.SlidingExpiration = true;
            options.EventsType = typeof(SqlAuthCookieEvents);
            //options.Validate();
         });

      services.TryAddSingleton<ISqlAuthPwdStore, SqlAuthPwdMemoryStore>();
      services.TryAddSingleton<ISqlAuthRuleValidator, SqlAuthRuleValidator>();
      services.TryAddSingleton<SqlAuthCookieEvents, SqlAuthCookieEvents>();

      return services.Configure(configureOptions);

      //return services;
   }

   public static IServiceCollection AddSQLConnAuthentication(this IServiceCollection services)
      => services.AddSqlConnAuthentication(options => { });

   public static IServiceCollection AddSqlConnAuthorization(this IServiceCollection services)
      => services.AddAuthorization(option => option.AddPolicy(SqlAuthConsts.SQLAUTHPOLICY,
         p => p.RequireAuthenticatedUser().AddAuthenticationSchemes(SqlAuthConsts.SQLAUTHSCHEME)));
}


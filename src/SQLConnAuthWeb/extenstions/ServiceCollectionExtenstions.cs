using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.passwords;
using Sorling.SqlConnAuthWeb.authentication.validation;
using Sorling.SqlConnAuthWeb.configuration;
using Sorling.SqlConnAuthWeb.razor;
using Sorling.SqlConnAuthWeb.authentication.dbaccess;

namespace Sorling.SqlConnAuthWeb.extenstions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure SQL connection authentication and authorization services.
/// </summary>
public static class ServiceCollectionExtenstions
{
   /// <summary>
   /// Adds SQL connection authentication services, cookie authentication, and related dependencies to the service collection.
   /// </summary>
   /// <param name="services">The service collection to configure.</param>
   /// <param name="sqlAuthPaths">The SQL authentication application path configuration.</param>
   /// <param name="configureOptions">An action to configure <see cref="SqlAuthOptions"/>.</param>
   /// <returns>The configured <see cref="IServiceCollection"/> instance.</returns>
   /// <exception cref="ArgumentNullException">Thrown if <paramref name="sqlAuthPaths"/> is null.</exception>
   public static IServiceCollection AddSqlConnAuthentication(this IServiceCollection services, SqlAuthAppPaths sqlAuthPaths
       , Action<SqlAuthOptions>? configureOptions = null) {
      ArgumentNullException.ThrowIfNull(sqlAuthPaths);

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
             options.Validate();
          });

      services.TryAddSingleton<ISqlAuthPwdStore, SqlAuthPwdMemoryStore>();
      services.TryAddSingleton<ISqlAuthRuleValidator, SqlAuthRuleValidator>();
      services.TryAddSingleton<SqlAuthCookieEvents, SqlAuthCookieEvents>();
      services.TryAddSingleton(sqlAuthPaths);
      services.TryAddSingleton<ISqlAuthPageRouteModelConvention, SqlAuthPageRouteModelConvention>();
      services.TryAddSingleton<ISqlAuthDBAccess, SqlAuthDBAccess>();

      // Register options from configuration and allow delegate override
      _ = services.AddOptions<SqlAuthOptions>()
          .Configure<IConfiguration>((opts, config) =>               // This will be handled by SqlAuthOptionsConfigurator for AllowedIPAddresses
                                                                     // But allow delegate to override
              configureOptions?.Invoke(opts));
      _ = services.AddSingleton<IConfigureOptions<SqlAuthOptions>, SqlAuthOptionsConfigurator>();

      return services;
   }

   /// <summary>
   /// Adds SQL connection authentication services with default options to the service collection.
   /// </summary>
   /// <param name="services">The service collection to configure.</param>
   /// <param name="sqlAuthPaths">The SQL authentication application path configuration.</param>
   /// <returns>The configured <see cref="IServiceCollection"/> instance.</returns>
   public static IServiceCollection AddSQLConnAuthentication(this IServiceCollection services, SqlAuthAppPaths sqlAuthPaths)
       => services.AddSqlConnAuthentication(sqlAuthPaths, null);

   /// <summary>
   /// Adds SQL connection authorization policy to the service collection.
   /// </summary>
   /// <param name="services">The service collection to configure.</param>
   /// <returns>The configured <see cref="IServiceCollection"/> instance.</returns>
   public static IServiceCollection AddSqlConnAuthorization(this IServiceCollection services)
       => services.AddAuthorization(option => option.AddPolicy(SqlAuthConsts.SQLAUTHPOLICY,
           p => p.RequireAuthenticatedUser().AddAuthenticationSchemes(SqlAuthConsts.SQLAUTHSCHEME)));
}


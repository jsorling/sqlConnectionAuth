using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.helpers;
using System.Net;

namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlAuthRuleValidator(IOptions<SqlAuthOptions> options) : ISqlAuthRuleValidator
{
   private readonly SqlAuthOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

   public async Task<SqlAuthRuleValidationResult> ValidateAsync(SqlAuthValidationRequest request) {
      if (!_options.AllowIntegratedSecurity && request.Password == SqlAuthConsts.WINDOWSAUTHENTICATION)
      {
         return new SqlAuthRuleValidationResult(new ApplicationException("Windows authentication not allowed"), null);
      }

      IPAddress[] ips;
      try
      {
         ips = await IPHelper.ResolveSqlIPAddressAsync(request.Datasource);
      }
      catch
      {
         return new SqlAuthRuleValidationResult(new ApplicationException("Unable to resolve SQL Server address"), null);
      }

      if (ips.Length == 0)
      {
         return new SqlAuthRuleValidationResult(new ApplicationException("No IP address found for SQL Server"), null);
      }

      if (!_options.AllowLoopbackConnections || !_options.AllowPrivateNetworkConnections)
      {
         foreach (IPAddress ip in ips)
         {
            IPNetworkType iptype = IPReservedNetworks.GetIPNetworkType(ip);

            if (iptype == IPNetworkType.Loopback && !_options.AllowLoopbackConnections)
            {
               return new SqlAuthRuleValidationResult(new ApplicationException("Loopback connections not allowed"), null);
            }
            else if (iptype != IPNetworkType.Public && !_options.AllowPrivateNetworkConnections)
            {
               return new SqlAuthRuleValidationResult(new ApplicationException("Private network connections not allowed"), null);
            }
         }
      }

      return new SqlAuthRuleValidationResult(null, new(
          Password: request.Password
          , TrustServerCertificate: request.TrustServerCertificate
          , RuleReValidationAfter: DateTime.UtcNow.AddMinutes(5)
      ));
   }
}


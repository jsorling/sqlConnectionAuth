using Sorling.SqlConnAuthWeb.authentication;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.extenstions;

internal static class ClaimsIdentityExtensions
{
   internal static SqlAuthCookieClaims? SqlConnAuthCookieClaims(this IEnumerable<ClaimsIdentity> ci) {
      ArgumentNullException.ThrowIfNull(ci);
      ClaimsIdentity? id = ci.FirstOrDefault(w => w.AuthenticationType == SqlAuthConsts.SQLAUTHSCHEME);

      return id is null ? null
         : new(id.Claims.Where(f => f.Type == SqlAuthConsts.CLAIMSQLSERVER).Select(s => s.Value).FirstOrDefault() ?? ""
         , id.Claims.Where(w => w.Type == SqlAuthConsts.CLAIMSQLUSERNAME).Select(s => s.Value).FirstOrDefault() ?? ""
         , id.Claims.Where(w => w.Type == SqlAuthConsts.CLAIMSQLPASSWORDREF).Select(s => s.Value).FirstOrDefault() ?? ""
         );
   }
}

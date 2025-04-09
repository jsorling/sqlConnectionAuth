using Sorling.SqlConnAuthWeb.authentication;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.extenstions;

internal static class ClaimsIdentityExtensions
{
   internal static SqlConnAuthCookieClaims? SqlConnAuthCookieClaims(this IEnumerable<ClaimsIdentity> ci) {
      ArgumentNullException.ThrowIfNull(ci);
      ClaimsIdentity? id = ci.FirstOrDefault(w => w.AuthenticationType == SqlConnAuthConsts.SQLCONNAUTHSCHEME);

      return id is null ? null
         : new(id.Claims.Where(f => f.Type == SqlConnAuthConsts.CLAIMSQLSERVER).Select(s => s.Value).FirstOrDefault() ?? ""
         , id.Claims.Where(w => w.Type == SqlConnAuthConsts.CLAIMSQLUSERNAME).Select(s => s.Value).FirstOrDefault() ?? ""
         , id.Claims.Where(w => w.Type == SqlConnAuthConsts.CLAIMSQLPASSWORDREF).Select(s => s.Value).FirstOrDefault() ?? ""
         );
   }
}

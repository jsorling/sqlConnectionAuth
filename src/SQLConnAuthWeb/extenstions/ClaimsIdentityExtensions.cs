using Sorling.SqlConnAuthWeb.authentication;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.extenstions;

internal static class ClaimsIdentityExtensions
{
   //internal static SQLConnAuthenticationData GetSQLConnAuthenticationData(this ClaimsIdentity ci)
   //   => new(((ci ?? throw new ArgumentNullException(nameof(ci)))
   //      .Claims.FirstOrDefault(f => f.Type == SQLConnAuthConsts.CLAIMSQLSERVER)
   //      ?? throw new ArgumentException($"Missing claim {SQLConnAuthConsts.CLAIMSQLSERVER}")).Value
   //      , (ci.Claims.FirstOrDefault(f => f.Type == SQLConnAuthConsts.CLAIMSQLDB)
   //      ?? throw new ArgumentException($"Missing claim {SQLConnAuthConsts.CLAIMSQLDB}")).Value
   //      , (ci.Claims.FirstOrDefault(f => f.Type == SQLConnAuthConsts.CLAIMSQLUSERNAME)
   //      ?? throw new ArgumentException($"Missing claim {SQLConnAuthConsts.CLAIMSQLUSERNAME}")).Value
   //      , (ci.Claims.FirstOrDefault(f => f.Type == SQLConnAuthConsts.CLAIMSQLPASSWORDGUID)
   //      ?? throw new ArgumentException($"Missing claim {SQLConnAuthConsts.CLAIMSQLPASSWORDGUID}")).Value);

   internal static SqlConnAuthenticationData? SQLConnAuthenticationData(this IEnumerable<ClaimsIdentity> ci) {
      if (ci is null)
         throw new ArgumentNullException(nameof(ci));
      ClaimsIdentity? id = ci.FirstOrDefault(w => w.AuthenticationType == SqlConnAuthConsts.SQLCONNAUTHSCHEME);

      return id is null ? null
         : new(id.Claims.Where(f => f.Type == SqlConnAuthConsts.CLAIMSQLSERVER).Select(s => s.Value).FirstOrDefault()
         , id.Claims.Where(w => w.Type == SqlConnAuthConsts.CLAIMSQLUSERNAME).Select(s => s.Value).FirstOrDefault()
         , id.Claims.Where(w => w.Type == SqlConnAuthConsts.CLAIMSQLPASSWORDREF).Select(s => s.Value).FirstOrDefault()
         );
   }
}

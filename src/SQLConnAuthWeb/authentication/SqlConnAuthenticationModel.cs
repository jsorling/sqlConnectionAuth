using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlConnAuthenticationModel
{
   [Required(AllowEmptyStrings = false, ErrorMessage = "Required: SQL server address")]
   [Display(Name = "SQL Server address")]
   public string? SqlServer { get; set; }

   //[Required(AllowEmptyStrings = false, ErrorMessage = "Required: database name")]
   //public string? SqlDatabase { get; set; }

   [Required(AllowEmptyStrings = false, ErrorMessage = "Required: password")]
   [DataType(DataType.Password)]
   public string? Password { get; set; }

   [Required(AllowEmptyStrings = false, ErrorMessage = "Required: user name")]
   public string? UserName { get; set; }

   public static SqlConnAuthenticationModel GetCurrent(ClaimsPrincipal cp) {
      SqlConnAuthenticationModel tor = new();
      if (cp != null) {
         ClaimsIdentity? ci = cp.Identities.FirstOrDefault(f => f.AuthenticationType == SqlConnAuthConsts.SQLCONNAUTHSCHEME);
         if (ci != null) {
            tor.SqlServer = ci.Claims.FirstOrDefault(f => f.Type == SqlConnAuthConsts.CLAIMSQLSERVER)?.Value;
            //tor.SqlDatabase = ci.Claims.FirstOrDefault(f => f.Type == SQLConnAuthConsts.CLAIMSQLDB)?.Value;
            tor.UserName = ci.Claims.FirstOrDefault(f => f.Type == SqlConnAuthConsts.CLAIMSQLUSERNAME)?.Value;
            tor.Password = ci.Claims.FirstOrDefault(f => f.Type == SqlConnAuthConsts.CLAIMSQLPASSWORDREF)?.Value;
         }
      }

      return tor;
   }
}

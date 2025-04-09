using Sorling.SqlConnAuthWeb.authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.razor.models;

public class InputAuthenticationModel
{
   [Required(AllowEmptyStrings = false, ErrorMessage = "Required: SQL server address")]
   [Display(Name = "SQL Server address")]
   public string SqlServer { get; set; } = "";

   [Required(AllowEmptyStrings = true, ErrorMessage = "Required: password")]
   [DataType(DataType.Password)]
   public string Password { get; set; } = "";

   [Required(AllowEmptyStrings = false, ErrorMessage = "Required: user name")]
   public string? UserName { get; set; }

   [Required]
   public bool TrustServerCertificate { get; set; }

   //public static InputAuthenticationModel GetCurrent(ClaimsPrincipal cp) {
   //   InputAuthenticationModel tor = new();
   //   if (cp != null) {
   //      ClaimsIdentity? ci = cp.Identities.FirstOrDefault(f => f.AuthenticationType == SqlConnAuthConsts.SQLCONNAUTHSCHEME);
   //      if (ci != null) {
   //         tor.SqlServer = ci.Claims.FirstOrDefault(f => f.Type == SqlConnAuthConsts.CLAIMSQLSERVER)?.Value;
   //         tor.UserName = ci.Claims.FirstOrDefault(f => f.Type == SqlConnAuthConsts.CLAIMSQLUSERNAME)?.Value;
   //         tor.Password = ci.Claims.FirstOrDefault(f => f.Type == SqlConnAuthConsts.CLAIMSQLPASSWORDREF)?.Value;
   //      }
   //   }

   //   return tor;
   //}

   public static implicit operator SqlConnAuthStoredSecrets(InputAuthenticationModel model)
      => new(model.Password, model.TrustServerCertificate);
}

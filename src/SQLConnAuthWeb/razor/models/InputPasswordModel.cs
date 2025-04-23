using Sorling.SqlConnAuthWeb.authentication;
using System.ComponentModel.DataAnnotations;

namespace Sorling.SqlConnAuthWeb.razor.models;

public class InputPasswordModel
{
   [Required(AllowEmptyStrings = true, ErrorMessage = "Required: password")]
   [DataType(DataType.Password)]
   public string Password { get; set; } = "";

   [Required]
   public bool TrustServerCertificate { get; set; }

   public static implicit operator SQLAuthenticateRequest(InputPasswordModel model)
      => new(Password: model.Password
          , TrustServerCertificate: model.TrustServerCertificate);
}

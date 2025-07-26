using Sorling.SqlConnAuthWeb.authentication;
using System.ComponentModel.DataAnnotations;

namespace Sorling.SqlConnAuthWeb.razor.models;

/// <summary>
/// Represents the input model for password-based SQL authentication in Razor Pages.
/// </summary>
public class InputPasswordModel
{
   /// <summary>
   /// Gets or sets the password for authentication.
   /// </summary>
   [Required(AllowEmptyStrings = true, ErrorMessage = "Required: password")]
   [DataType(DataType.Password)]
   public string Password { get; set; } = "";

   /// <summary>
   /// Gets or sets a value indicating whether to trust the SQL Server certificate.
   /// </summary>
   [Required]
   public bool TrustServerCertificate { get; set; }

   [Required]
   [Display(Name = "Bogus")]
   public string Bogus { get; set; } = "This is a bogus property to ensure the model is not empty.";

   /// <summary>
   /// Implicitly converts an <see cref="InputPasswordModel"/> to a <see cref="SQLAuthenticateRequest"/>.
   /// </summary>
   /// <param name="model">The input password model to convert.</param>
   public static implicit operator SQLAuthenticateRequest(InputPasswordModel model)
       => new(Password: model.Password
           , TrustServerCertificate: model.TrustServerCertificate);
}

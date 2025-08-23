using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.authentication.passwords;
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
   /// <value>
   /// The password string used for SQL authentication.
   /// </value>
   [Required(AllowEmptyStrings = true, ErrorMessage = "Required: password")]
   [DataType(DataType.Password)]
   public string Password { get; set; } = "";

   /// <summary>
   /// Gets or sets a value indicating whether to trust the SQL Server certificate.
   /// </summary>
   /// <value>
   /// <c>true</c> to trust the server certificate; otherwise, <c>false</c>.
   /// </value>
   [Required]
   public bool TrustServerCertificate { get; set; }

   /// <summary>
   /// Implicitly converts an <see cref="InputPasswordModel"/> to a <see cref="SQLAuthenticateRequest"/>.
   /// </summary>
   /// <param name="model">The input password model to convert.</param>
   /// <returns>A <see cref="SQLAuthenticateRequest"/> instance with values from the model.</returns>
   public static implicit operator SQLAuthenticateRequest(InputPasswordModel model)
       => new(Password: model.Password
           , TrustServerCertificate: model.TrustServerCertificate);

   /// <summary>
   /// Implicitly converts an <see cref="InputPasswordModel"/> to a <see cref="SqlAuthTempPasswordInfo"/>.
   /// </summary>
   /// <param name="model">The input password model to convert.</param>
   /// <returns>A <see cref="SqlAuthTempPasswordInfo"/> instance with values from the model.</returns>
   public static implicit operator SqlAuthTempPasswordInfo(InputPasswordModel model)
       => new(model.Password, model.TrustServerCertificate);
}

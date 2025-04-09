using System.ComponentModel.DataAnnotations;

namespace Sorling.SqlConnAuthWeb.razor.models;

public class InputServerNameModel
{
   [Required(AllowEmptyStrings = false, ErrorMessage = "Required: SQL server address")]
   [Display(Name = "SQL Server address")]
   public string? SqlServer { get; set; }

   [Required(AllowEmptyStrings = false, ErrorMessage = "Required: user name")]
   [Display(Name = "User name")]
   public string? UserName { get; set; }
}

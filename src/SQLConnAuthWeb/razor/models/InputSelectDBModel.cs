using System.ComponentModel.DataAnnotations;

namespace Sorling.SqlConnAuthWeb.razor.models;

public class InputSelectDBModel
{
   [Required]
   public string DBName { get; set; } = string.Empty;
}

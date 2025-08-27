using Sorling.SqlConnAuthWeb.authentication.dbaccess;
using System.ComponentModel.DataAnnotations;

namespace Sorling.SqlConnAuthWeb.razor.models;

/// <summary>
/// Represents the input model for selecting a database in the SQL authentication UI.
/// </summary>
public class InputSelectDBModel
{
   /// <summary>
   /// Gets or sets the selected database name.
   /// </summary>
   [Required]
   public string DBName { get; set; } = string.Empty;

   public IEnumerable<ISqlDatabase> Databases { get; set; } = [];
}

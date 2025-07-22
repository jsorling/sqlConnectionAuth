using System.ComponentModel.DataAnnotations;

namespace Sorling.SqlConnAuthWeb.razor.models;

/// <summary>
/// Represents the input model for entering a SQL Server address and user name in Razor Pages.
/// </summary>
public class InputServerNameModel
{
    /// <summary>
    /// Gets or sets the SQL Server address.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Required: SQL server address")]
    [Display(Name = "SQL Server address")]
    public string SqlServer { get; set; } = "";

    /// <summary>
    /// Gets or sets the user name for SQL authentication.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Required: user name")]
    [Display(Name = "User name")]
    public string UserName { get; set; } = "";
}

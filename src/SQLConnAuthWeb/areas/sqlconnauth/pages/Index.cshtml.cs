using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.razor.models;
using System.ComponentModel.DataAnnotations;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

[AllowAnonymous]
[RequireHttps]
public class IndexModel(ISqlConnAuthenticationService sqlConnAuthenticationService) : PageModel
{
   protected readonly ISqlConnAuthenticationService _sqlauth = sqlConnAuthenticationService;

   public SqlConnAuthenticationOptions SqlConnAuthenticationOptions => _sqlauth.Options;

   [BindProperty]
   public InputServerNameModel Input { get; set; } = new();

   public IActionResult OnGet() => Page();

   public IActionResult OnPost() => ModelState.IsValid
      ? Redirect(_sqlauth.UriEscapedPath(Input.SqlServer, Input.UserName))
      : Page();
}

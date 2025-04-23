using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.areas.sqlconnauth.pages;

public class ThemeSwitcherJS(IOptions<SqlAuthOptions> options) : PageModel
{
   public SqlAuthOptions SQLAuthOptions { get; } = options.Value ?? throw new ArgumentNullException(nameof(options));

   public IActionResult OnGet() => Page();
}

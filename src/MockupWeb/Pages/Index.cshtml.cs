using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MockupWeb.Pages;

[AllowAnonymous]
[RequireHttps]
public class IndexModel : PageModel
{
   public void OnGet() {

   }
}

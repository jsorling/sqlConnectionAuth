using Microsoft.AspNetCore.Mvc.RazorPages;
using Sorling.SqlConnAuthWeb.authentication;

namespace MockupWeb.Pages;

public class IndexModel(ILogger<IndexModel> logger, ISqlConnAuthenticationService sqlConnAuthenticationService) : PageModel
{
   private readonly ILogger<IndexModel> _logger = logger;

   public readonly ISqlConnAuthenticationService SqlConnAuthentication = sqlConnAuthenticationService;

   public void OnGet() {

   }
}

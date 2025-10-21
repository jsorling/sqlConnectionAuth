using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag Helper that preserves ambient SQL authentication route parameters when generating
/// links to Razor Pages using the <c>asp-page</c> attribute on <c>&lt;a&gt;</c> tags.
/// <para>
/// If the current request contains any of the SQL auth route values
/// (<see cref="SqlAuthConsts.URLROUTEPARAMSRV"/>, <see cref="SqlAuthConsts.URLROUTEPARAMUSR"/>,
/// <see cref="SqlAuthConsts.URLROUTEPARAMDB"/>) or an ambient <c>area</c>, and the corresponding
/// <c>asp-route-*</c> or <c>asp-area</c> attributes are not explicitly set on the anchor,
/// they are added so that generated URLs include those values.
/// </para>
/// </summary>
public class SqlAuthAnchorRouteTagHelper : TagHelper
{
   /// <summary>
   /// Ensures this Tag Helper runs before the built-in <c>AnchorTagHelper</c> so added attributes are
   /// considered during URL generation.
   /// </summary>
   public override int Order => -1000;

   /// <summary>
   /// The current <see cref="ViewContext"/> injected by the framework.
   /// Used to access the active <see cref="Microsoft.AspNetCore.Http.HttpContext"/> and ambient route values.
   /// </summary>
   [HtmlAttributeNotBound]
   [ViewContext]
   public ViewContext ViewContext { get; set; } = default!;

   /// <summary>
   /// Adds missing <c>asp-area</c> and <c>asp-route-*</c> attributes based on ambient SQL auth route values.
   /// </summary>
   /// <param name="context">The tag helper context.</param>
   /// <param name="output">The tag helper output to modify.</param>
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      if (ViewContext?.HttpContext is null)
      {
         return;
      }

      RouteValueDictionary ambient = razor.SqlAuthRouteHelper.GetSqlAuthRouteValues(ViewContext.HttpContext);

      // Preserve area via asp-area if not already specified
      if (ambient.TryGetValue("area", out object? area) && area is not null && !output.Attributes.ContainsName("asp-area"))
      {
         output.Attributes.SetAttribute("asp-area", area);
      }

      // Preserve SQL auth params via asp-route-*
      void TrySet(string key) {
         if (ambient.TryGetValue(key, out object? val) && val is not null)
         {
            string attrname = $"asp-route-{key}";
            if (!output.Attributes.ContainsName(attrname))
            {
               output.Attributes.SetAttribute(attrname, val);
            }
         }
      }

      TrySet(SqlAuthConsts.URLROUTEPARAMSRV);
      TrySet(SqlAuthConsts.URLROUTEPARAMUSR);
      TrySet(SqlAuthConsts.URLROUTEPARAMDB);
   }
}

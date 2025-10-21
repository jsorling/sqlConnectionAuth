using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag helper that emits a link tag for the ThemeSwitcher CSS page with cache busting, using the SQLAUTHAREA constant.
/// </summary>
[HtmlTargetElement("sql-auth-themeswitcher-css", TagStructure = TagStructure.WithoutEndTag)]
public class SqlAuthThemeSwitcherCssTagHelper(OptionsVersionProvider versionProvider) : TagHelper
{
   /// <summary>
   /// Processes the tag helper and writes a link tag referencing the Theme Switcher CSS endpoint with a version query string.
   /// </summary>
   /// <param name="context">Contextual information about the tag helper.</param>
   /// <param name="output">The target element output to write to.</param>
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      output.TagName = "link";
      output.TagMode = TagMode.SelfClosing;
      string href = $"/{SqlAuthConsts.SQLAUTHAREA}/themeswitchercss?v={versionProvider.Version}";
      output.Attributes.SetAttribute("rel", "stylesheet");
      output.Attributes.SetAttribute("href", href);
      output.Attributes.SetAttribute("type", "text/css");
   }
}

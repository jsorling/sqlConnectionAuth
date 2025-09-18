using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag helper that emits a link tag for the ThemeSwitcher CSS page with cache busting, using the SQLAUTHAREA constant.
/// </summary>
[HtmlTargetElement("sql-auth-themeswitcher-css", TagStructure = TagStructure.WithoutEndTag)]
public class SqlAuthThemeSwitcherCssTagHelper(OptionsVersionProvider versionProvider) : TagHelper
{
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      output.TagName = "link";
      output.TagMode = TagMode.SelfClosing;
      string href = $"/{SqlAuthConsts.SQLAUTHAREA}/themeswitchercss?v={versionProvider.Version}";
      output.Attributes.SetAttribute("rel", "stylesheet");
      output.Attributes.SetAttribute("href", href);
      output.Attributes.SetAttribute("type", "text/css");
   }
}

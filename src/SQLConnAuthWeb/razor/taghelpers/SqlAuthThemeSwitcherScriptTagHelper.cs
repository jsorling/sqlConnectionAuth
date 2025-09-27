using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag helper that emits a script tag for the ThemeSwitcher JS page with cache busting, using the SQLAUTHAREA constant.
/// </summary>
[HtmlTargetElement("sql-auth-themeswitcher-script", TagStructure = TagStructure.WithoutEndTag)]
public class SqlAuthThemeSwitcherScriptTagHelper(OptionsVersionProvider versionProvider) : TagHelper
{
   /// <summary>
   /// Processes the tag helper and writes a script tag referencing the Theme Switcher JS endpoint with a version query string.
   /// </summary>
   /// <param name="context">Contextual information about the tag helper.</param>
   /// <param name="output">The target element output to write to.</param>
   //@addTagHelper *, Sorling.SqlConnAuthWeb
   //<sql-auth-themeswitcher-script />
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      output.TagName = "script";
      output.TagMode = TagMode.StartTagAndEndTag;
      string src = $"/{SqlAuthConsts.SQLAUTHAREA}/themeswitcher?v={versionProvider.Version}";
      output.Attributes.SetAttribute("src", src);
      output.Attributes.SetAttribute("type", "text/javascript");
   }
}

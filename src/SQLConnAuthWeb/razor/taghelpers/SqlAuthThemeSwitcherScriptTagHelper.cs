using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag helper that emits a script tag for the ThemeSwitcher JS page with cache busting, using the SQLAUTHAREA constant.
/// </summary>
[HtmlTargetElement("sql-auth-themeswitcher-script", TagStructure = TagStructure.WithoutEndTag)]
public class SqlAuthThemeSwitcherScriptTagHelper(OptionsVersionProvider versionProvider) : TagHelper
{
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

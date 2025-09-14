using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag helper that emits a script tag for the SQL Auth JS page with cache busting, using the SQLAUTHAREA constant.
/// </summary>
[HtmlTargetElement("sql-auth-js-script", TagStructure = TagStructure.WithoutEndTag)]
public class SqlAuthJsScriptTagHelper(OptionsVersionProvider versionProvider) : TagHelper
{
   //@addTagHelper *, Sorling.SqlConnAuthWeb
   //<sql-auth-js-script />
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      output.TagName = "script";
      output.TagMode = TagMode.StartTagAndEndTag;
      string src = $"/{SqlAuthConsts.SQLAUTHAREA}/js?v={versionProvider.Version}";
      output.Attributes.SetAttribute("src", src);
      output.Attributes.SetAttribute("type", "text/javascript");
   }
}

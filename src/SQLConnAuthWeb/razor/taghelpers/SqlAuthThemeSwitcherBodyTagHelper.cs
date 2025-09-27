using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Renders the theme switcher icon container markup if the feature is enabled.
/// </summary>
[HtmlTargetElement("sql-auth-themeswitcher-body", TagStructure = TagStructure.WithoutEndTag)]
public class SqlAuthThemeSwitcherBodyTagHelper(IOptionsMonitor<SqlAuthUIOptions> uiOptions) : TagHelper
{
   /// <summary>
   /// Processes the tag helper and writes the theme switcher markup if enabled; otherwise suppresses output.
   /// </summary>
   /// <param name="context">Contextual information about the tag helper.</param>
   /// <param name="output">The target element output to write to.</param>
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      SqlAuthUIOptions opts = uiOptions.CurrentValue;
      if (!opts.UseThemeSwitcher())
      {
         output.SuppressOutput();
         return;
      }

      output.TagName = null; // emit raw markup
      _ = output.Content.SetHtmlContent("""
<div class="theme-switcher-icon-container" id="theme-switcher-icon-container" title="Switch theme">
        <span class="theme-switcher-icon" id="theme-switcher-icon">
            <!-- Default: system icon (monitor) -->
            <svg id="theme-icon-system" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" style="display:inline;"><rect x="3" y="5" width="18" height="12" rx="2" fill="currentColor"/><rect x="7" y="19" width="10" height="2" rx="1" fill="currentColor"/></svg>
        </span>
</div>

""");
   }
}

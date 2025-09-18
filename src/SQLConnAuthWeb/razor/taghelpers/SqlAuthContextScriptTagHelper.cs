using Microsoft.AspNetCore.Razor.TagHelpers;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag helper that emits a script tag exposing SqlAuthContext data to JavaScript as window.SqlAuthContext.
/// </summary>
[HtmlTargetElement("sql-auth-context-script", TagStructure = TagStructure.WithoutEndTag)]
public class SqlAuthContextScriptTagHelper(ISqlAuthContext context) : TagHelper
{
   private readonly ISqlAuthContext _context = context;

   //@addTagHelper *, Sorling.SqlConnAuthWeb
   //<sql-auth-context-script />
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      output.TagName = "script";
      output.TagMode = TagMode.StartTagAndEndTag;
      _ = output.Content.SetHtmlContent($"window.SqlAuthContext = {{SqlServer: {FormatNullable(_context.SqlServer)}, SqlUserName: {FormatNullable(_context.SqlUserName)}, SqlDBName: {FormatNullable(_context.SqlDBName)}}};");
   }

   private static string Escape(string value) => value?.Replace("'", "\\'") ?? string.Empty;

   private static string FormatNullable(string? value) => value == null ? "null" : $"'" + Escape(value) + "'";
}

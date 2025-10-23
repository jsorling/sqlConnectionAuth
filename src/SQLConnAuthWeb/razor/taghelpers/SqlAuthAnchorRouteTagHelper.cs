using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Sorling.SqlConnAuthWeb.razor.taghelpers;

/// <summary>
/// Tag Helper that preserves ambient SQL authentication route parameters when generating
/// links to Razor Pages using the <c>asp-page</c> attribute on <c>&lt;a&gt;</c> tags.
/// <para>
/// This helper runs after the built-in <c>AnchorTagHelper</c> and:
/// </para>
/// <list type="bullet">
/// <item><description>Detects whether the current request is within the SQL authentication ambient context (i.e., has <see cref="SqlAuthConsts.URLROUTEPARAMSRV"/> and/or <see cref="SqlAuthConsts.URLROUTEPARAMUSR"/> route values).</description></item>
/// <item><description>If the built-in anchor processing could not resolve an <c>href</c> (left empty), it computes a URL using <see cref="Microsoft.AspNetCore.Routing.LinkGenerator"/> by merging existing <c>asp-route-*</c> values with ambient SQL auth route values.</description></item>
/// <item><description>Removes all <c>asp-*</c> attributes from the final output so they never render literally.</description></item>
/// </list>
/// <remarks>
/// - When <c>UseDBNameRouting</c> is disabled, <see cref="SqlAuthConsts.URLROUTEPARAMDB"/> is not required and will not be added.
/// - The helper intentionally does not modify anchors outside the SQL auth ambient context.
/// </remarks>
/// </summary>
[HtmlTargetElement("a", Attributes = "asp-page")]
[HtmlTargetElement("a", Attributes = "asp-page-handler")]
[HtmlTargetElement("a", Attributes = "asp-route-*")]
public class SqlAuthAnchorRouteTagHelper : TagHelper
{
   /// <summary>
   /// Runs after the built-in <c>AnchorTagHelper</c> (positive order) so we can fix unresolved links
   /// and then strip any remaining <c>asp-*</c> attributes before rendering.
   /// </summary>
   public override int Order => 1000;

   /// <summary>
   /// The current <see cref="ViewContext"/> provided by the Razor Pages runtime.
   /// Used to access the active <see cref="Microsoft.AspNetCore.Http.HttpContext"/>, ambient route values,
   /// and the current page path for resolving relative <c>asp-page</c> values.
   /// </summary>
   [HtmlAttributeNotBound]
   [ViewContext]
   public ViewContext ViewContext { get; set; } = default!;

   /// <summary>
   /// Processes the <c>&lt;a&gt;</c> element:
   /// <list type="number">
   /// <item><description>Skips processing when not in SQL auth ambient context (neither server nor user present).</description></item>
   /// <item><description>If the built-in anchor could not produce an <c>href</c>, computes it using <see cref="LinkGenerator"/>.</description></item>
   /// <item><description>Removes all remaining <c>asp-*</c> attributes from the output.</description></item>
   /// </list>
   /// </summary>
   /// <param name="context">The tag helper context.</param>
   /// <param name="output">The tag helper output to modify.</param>
   public override void Process(TagHelperContext context, TagHelperOutput output) {
      if (ViewContext?.HttpContext is null)
      {
         return;
      }

      // Detect ambient SQL auth context early; if not present, do nothing.
      RouteValueDictionary ambient = razor.SqlAuthRouteHelper.GetSqlAuthRouteValues(ViewContext.HttpContext);
      bool hassrv = ambient.TryGetValue(SqlAuthConsts.URLROUTEPARAMSRV, out object? srv) && srv is string s1 && !string.IsNullOrEmpty(s1);
      bool hasusr = ambient.TryGetValue(SqlAuthConsts.URLROUTEPARAMUSR, out object? usr) && usr is string s2 && !string.IsNullOrEmpty(s2);
      if (!hassrv && !hasusr)
      {
         // Not in SQL auth ambient context; let prior helpers' output stand.
         return;
      }

      // If AnchorTagHelper failed (href empty), compute href ourselves using LinkGenerator
      string? existinghref = output.Attributes["href"]?.Value?.ToString();
      bool needsfix = string.IsNullOrEmpty(existinghref);

      if (needsfix)
      {
         string? pageattr = context.AllAttributes["asp-page"]?.Value?.ToString();
         string? handler = context.AllAttributes["asp-page-handler"]?.Value?.ToString();

         if (!string.IsNullOrEmpty(pageattr))
         {
            // Collect existing asp-route-* values from the tag
            RouteValueDictionary routevalues = [];
            foreach (TagHelperAttribute attr in context.AllAttributes)
            {
               if (attr.Name.StartsWith("asp-route-", StringComparison.Ordinal))
               {
                  string key = attr.Name["asp-route-".Length..];
                  if (!string.IsNullOrEmpty(key))
                  {
                     routevalues[key] = attr.Value?.ToString();
                  }
               }
            }

            // Merge ambient SQL auth params from the current request if not already present
            void TryMerge(string key) {
               if (!routevalues.ContainsKey(key) && ambient.TryGetValue(key, out object? v) && v is not null)
               {
                  routevalues[key] = v.ToString();
               }
            }

            // Always try to merge server and user; DB is optional and only merged when present
            TryMerge(SqlAuthConsts.URLROUTEPARAMSRV);
            TryMerge(SqlAuthConsts.URLROUTEPARAMUSR);
            TryMerge(SqlAuthConsts.URLROUTEPARAMDB);

            string resolvedpage = ResolvePagePath(pageattr);
            LinkGenerator? linkgen = ViewContext.HttpContext.RequestServices.GetService(typeof(LinkGenerator)) as LinkGenerator;
            if (linkgen is not null)
            {
               string? href = linkgen.GetPathByPage(ViewContext.HttpContext, resolvedpage, handler, routevalues);
               if (!string.IsNullOrEmpty(href))
               {
                  output.Attributes.SetAttribute("href", href);
               }
            }
         }
      }

      // Strip asp-* attributes from final output to avoid rendering them
      List<string> toremove = [];
      foreach (TagHelperAttribute attr in output.Attributes)
      {
         if (attr.Name.StartsWith("asp-", StringComparison.Ordinal))
         {
            toremove.Add(attr.Name);
         }
      }

      foreach (string name in toremove)
      {
         _ = output.Attributes.RemoveAll(name);
      }
   }

   /// <summary>
   /// Resolves the effective Razor Pages path used with <see cref="Microsoft.AspNetCore.Routing.LinkGenerator"/> (for example, when calling GetPathByPage)
   /// from a value provided via the <c>asp-page</c> attribute.
   /// </summary>
   /// <param name="page">
   /// The <c>asp-page</c> value. Can be absolute (e.g. "/db/import/dl") or relative (e.g. "./dl" or "../select").
   /// </param>
   /// <returns>
   /// An absolute Razor Pages path beginning with '/'.
   /// </returns>
   /// <remarks>
   /// Relative values are resolved using the current page's directory from <see cref="PageActionDescriptor.ViewEnginePath"/>.
   /// </remarks>
   private string ResolvePagePath(string page) {
      // Absolute page path
      if (page.StartsWith('/'))
      {
         return page;
      }

      // Base directory of current page
      string currentpage = (ViewContext.ActionDescriptor as PageActionDescriptor)?.ViewEnginePath ?? string.Empty; // e.g. "/db/import/upload"
      string basedir = string.Empty;
      int lastslash = currentpage.LastIndexOf('/');
      if (lastslash >= 0)
      {
         basedir = currentpage[..lastslash]; // e.g. "/db/import"
      }

      // Split and normalize segments
      static IEnumerable<string> SplitSegments(string p) => p.Split('/', StringSplitOptions.RemoveEmptyEntries);

      List<string> segments = [.. SplitSegments(basedir)];
      string work = page;

      while (work.StartsWith("./", StringComparison.Ordinal))
      {
         work = work[2..];
      }

      while (work.StartsWith("../", StringComparison.Ordinal))
      {
         if (segments.Count > 0)
            segments.RemoveAt(segments.Count - 1);
         work = work[3..];
      }

      foreach (string seg in SplitSegments(work))
      {
         if (seg == ".")
            continue;
         if (seg == "..")
         { if (segments.Count > 0) segments.RemoveAt(segments.Count - 1); continue; }

         segments.Add(seg);
      }

      return "/" + string.Join('/', segments);
   }
}

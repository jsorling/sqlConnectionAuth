using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing.Patterns;

namespace MockupWeb.Pages;

[AllowAnonymous]
[RequireHttps]
public class IndexModel(EndpointDataSource endpointDataSource) : PageModel
{
   public List<EndpointGroup> EndpointGroups { get; private set; } = [];

   public void OnGet() {
      List<EndpointInfo> endpoints = [.. endpointDataSource.Endpoints
          .Select(e => {
             string? area = null;
             string? routepattern = (e as RouteEndpoint)?.RoutePattern?.RawText;
             List<AttributeInfo> attributes = e.Metadata?.Select(m => new AttributeInfo {
                Name = m.GetType().Name,
                Description = GetAttributeDescription(m)
             }).ToList() ?? [];
             if (e is RouteEndpoint re)
             {
                if (re.RoutePattern.Defaults.TryGetValue("area", out object? areaobj) && areaobj is string areastr)
                {
                   area = areastr;
                }
                else
                {
                   RoutePatternParameterPart? areaparam = re.RoutePattern.Parameters.FirstOrDefault(p => p.Name == "area");
                   if (areaparam != null)
                   {
                      area = "(area route param)";
                   }
                }
             }

             return new EndpointInfo {
                DisplayName = e.DisplayName,
                RoutePattern = routepattern,
                HttpMethods = e.Metadata!
                       .OfType<HttpMethodMetadata>()
                       .FirstOrDefault()?.HttpMethods,
                Area = area,
                Attributes = attributes
             };
          })];

      // Group by area and display name
      EndpointGroups = [.. endpoints
          .GroupBy(e => new {
             e.Area,
             e.DisplayName
          })
          .OrderBy(g => g.Key.Area ?? "")
          .ThenBy(g => g.Key.DisplayName ?? "")
          .Select(g => new EndpointGroup {
             Area = g.Key.Area,
             DisplayName = g.Key.DisplayName,
             Endpoints = [.. g.OrderBy(e => e.RoutePattern ?? "")]
          })];
   }

   // Helper to get a description string for common attributes
   private static string GetAttributeDescription(object attr) {
      switch (attr)
      {
         case HttpMethodMetadata http:
            return $"Methods: {string.Join(", ", http.HttpMethods)}";
         case AuthorizeAttribute auth:
            List<string> parts = [];
            if (!string.IsNullOrEmpty(auth.Policy))
               parts.Add($"Policy: {auth.Policy}");
            if (!string.IsNullOrEmpty(auth.Roles))
               parts.Add($"Roles: {auth.Roles}");
            if (!string.IsNullOrEmpty(auth.AuthenticationSchemes))
               parts.Add($"Schemes: {auth.AuthenticationSchemes}");
            return string.Join("; ", parts);
         case AllowAnonymousAttribute:
            return "Allows anonymous access";
         case RequireHttpsAttribute:
            return "Requires HTTPS";
         case Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.PageModelAttribute pagemodelattr:
            System.Reflection.PropertyInfo? handlertypeprop = pagemodelattr.GetType().GetProperty("HandlerType");
            Type? handlertype = handlertypeprop?.GetValue(pagemodelattr) as Type;
            return handlertype?.Name ?? handlertype?.FullName ?? string.Empty;
         default:
            // Handle CompiledPageActionDescriptor via reflection
            Type type = attr.GetType();
            if (type.FullName == "Microsoft.AspNetCore.Mvc.RazorPages.CompiledPageActionDescriptor")
            {
               System.Reflection.PropertyInfo? modeltypeprop = type.GetProperty("ModelType");
               Type? modeltype = modeltypeprop?.GetValue(attr) as Type;
               System.Reflection.PropertyInfo? viewenginepathprop = type.GetProperty("ViewEnginePath");
               string? viewenginepath = viewenginepathprop?.GetValue(attr) as string;
               System.Reflection.PropertyInfo? relativepathprop = type.GetProperty("RelativePath");
               string? relativepath = relativepathprop?.GetValue(attr) as string;
               List<string> details = [];
               if (modeltype != null)
                  details.Add($"Model: {modeltype.Name}");
               if (!string.IsNullOrEmpty(viewenginepath))
                  details.Add($"ViewEnginePath: {viewenginepath}");
               if (!string.IsNullOrEmpty(relativepath))
                  details.Add($"RelativePath: {relativepath}");
               return string.Join("; ", details);
            }

            return string.Empty;
      }
   }

   public class AttributeInfo
   {
      public string Name { get; set; } = string.Empty;
      public string Description { get; set; } = string.Empty;
   }

   public class EndpointInfo
   {
      public string? DisplayName { get; set; }
      public string? RoutePattern { get; set; }
      public IEnumerable<string>? HttpMethods { get; set; }
      public string? Area { get; set; }
      public List<AttributeInfo> Attributes { get; set; } = [];
   }

   public class EndpointGroup
   {
      public string? Area { get; set; }
      public string? DisplayName { get; set; }
      public List<EndpointInfo> Endpoints { get; set; } = [];
   }
}

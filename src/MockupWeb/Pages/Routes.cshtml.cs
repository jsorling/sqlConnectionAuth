using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MockupWeb.Pages;

public class RoutesModel : PageModel
{
   public void OnGet(EndpointDataSource endpointSource) {

      Endpoint x = endpointSource.Endpoints.Single();
      //var sb = new StringBuilder();
      //var endpoints = endpointSource.Endpoints.SelectMany(es => es.Endpoints);
      //foreach (var endpoint in endpoints) {
      //   if (endpoint is RouteEndpoint routeEndpoint) {
      //      _ = routeEndpoint.RoutePattern.RawText;
      //      _ = routeEndpoint.RoutePattern.PathSegments;
      //      _ = routeEndpoint.RoutePattern.Parameters;
      //      _ = routeEndpoint.RoutePattern.InboundPrecedence;
      //      _ = routeEndpoint.RoutePattern.OutboundPrecedence;
      //   }

      //   var routeNameMetadata = endpoint.Metadata.OfType<Microsoft.AspNetCore.Routing.RouteNameMetadata>().FirstOrDefault();
      //   _ = routeNameMetadata?.RouteName;

      //   var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
      //   _ = httpMethodsMetadata?.HttpMethods; // [GET, POST, ...]

      //   // There are many more metadata types available...
      //}      
   }
}

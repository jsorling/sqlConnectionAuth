using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// A Razor Pages route model convention that rewrites page routes to include SQL authentication parameters (server and user) in the path.
/// </summary>
/// <param name="path">The SQL authentication application path configuration.</param>
public class SqlAuthPageRouteModelConvention(SqlAuthAppPaths path) : IPageRouteModelConvention
{
   private readonly SqlAuthAppPaths _path = path;

   /// <summary>
   /// Applies the route convention to the specified <see cref="PageRouteModel"/>, rewriting routes to include server and user parameters.
   /// </summary>
   /// <param name="model">The page route model to modify.</param>
   public void Apply(PageRouteModel model) {
      string p = _path.Root.TrimStart('/').TrimEnd('/');
      foreach (SelectorModel selector in model.Selectors.Where(w => SelectorTemplateMatch(w, p)))
      {
         string? newtemplate = null;
         if (selector.AttributeRouteModel!.Template!.StartsWith(p + "/")
             || selector.AttributeRouteModel.Template == p)
            newtemplate = $"/{p}/{{{SqlAuthConsts.URLROUTEPARAMSRV}}}/{{{SqlAuthConsts.URLROUTEPARAMUSR}}}"
                + (string.IsNullOrWhiteSpace(_path.Tail.Trim('/')) ? "/" : "/" + _path.Tail.Trim('/') + "/")
                + selector.AttributeRouteModel.Template[p.Length..].TrimStart('/');

         if (newtemplate is not null)
         {
            selector.AttributeRouteModel.Template = newtemplate;
         }
      }
   }

   //private string RewriteTemplate(string template, string path, string tailPath) {
   //   string tp = template?.TrimStart('/') ?? string.Empty;



   //   string tor = string.IsNullOrWhiteSpace(tailPath)
   //      ? template.StartsWith(path + "/")
   //         ? template[path.Length..].TrimStart('/')
   //         : template == path
   //            ? string.Empty
   //            : template
   //      : template.StartsWith(path + "/")
   //         ? $"{template[path.Length..].TrimStart('/')}{tailPath}"
   //         : template == path
   //            ? tailPath
   //            : $"{template}{tailPath}";
   //}
      //=> template.StartsWith(path + "/")
      //   ? template[path.Length..].TrimStart('/')
      //   : template == path
      //      ? string.Empty
      //      : template;

   private static bool SelectorTemplateMatch(SelectorModel selectorModel, string path)
      => selectorModel.AttributeRouteModel is not null
         && selectorModel.AttributeRouteModel.Template is not null
         && (selectorModel.AttributeRouteModel.Template.StartsWith(path + "/")
            || selectorModel.AttributeRouteModel.Template == path);
}

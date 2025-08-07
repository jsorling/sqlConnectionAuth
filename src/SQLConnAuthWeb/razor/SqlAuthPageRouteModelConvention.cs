using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.helpers;
using System.Diagnostics.CodeAnalysis;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// A Razor Pages route model convention that rewrites page routes to include SQL authentication parameters (server and user) in the path.
/// </summary>
/// <param name="path">The SQL authentication application path configuration.</param>
public class SqlAuthPageRouteModelConvention(SqlAuthAppPaths path) : ISqlAuthPageRouteModelConvention {
   private readonly RouteTemplateBuilder _templateBuilder = new(path ?? throw new ArgumentNullException(nameof(path)));

   /// <summary>
   /// Applies the route convention to the specified <see cref="PageRouteModel"/>, rewriting routes to include server and user parameters.
   /// </summary>
   /// <param name="model">The page route model to modify.</param>
   public void Apply(PageRouteModel model) {
      foreach (SelectorModel selector in model.Selectors.Where(w => SelectorTemplateMatch(w)))
      {
         selector!.AttributeRouteModel!.Template = _templateBuilder.BuildTemplate(selector!.AttributeRouteModel!.Template!);
      }
   }

   [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>")]
   private bool SelectorTemplateMatch(SelectorModel selectorModel) {
      if (selectorModel.AttributeRouteModel is null || selectorModel.AttributeRouteModel.Template is null) {
         return false; 
      }

      if (_templateBuilder.RootIsTopLevel)
      { 
         // match any template
         return true;
      }

      if (_templateBuilder.RootIsTopLevelFolder 
         && (selectorModel.AttributeRouteModel.Template.Trim('/') == _templateBuilder.EffectiveRoot.Trim('/')
            || selectorModel.AttributeRouteModel.Template.TrimStart('/').StartsWith(_templateBuilder.EffectiveRoot.TrimStart('/') + "/")))
      {
         return true; 
      }

      return selectorModel.AttributeRouteModel.Template.Trim('/').StartsWith(_templateBuilder.EffectiveRoot);
   }
}

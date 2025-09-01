using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.helpers;
using System.Diagnostics.CodeAnalysis;

namespace Sorling.SqlConnAuthWeb.razor;

/// <summary>
/// A Razor Pages route model convention that rewrites page routes to include SQL authentication parameters (server and user) in the path.
/// </summary>
/// <param name="path">The SQL authentication application path configuration.</param>
public class SqlAuthPageRouteModelConvention(SqlAuthAppPaths path) : ISqlAuthPageRouteModelConvention
{
   /// <summary>
   /// The route template builder used to construct route templates with SQL authentication parameters.
   /// </summary>
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

   /// <summary>
   /// Determines whether the selector's route template matches the root configuration for SQL authentication.
   /// </summary>
   /// <param name="sModel">The selector model to evaluate.</param>
   /// <returns><c>true</c> if the selector's template matches the root configuration; otherwise, <c>false</c>.</returns>
   [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
   private bool SelectorTemplateMatch(SelectorModel sModel) {
      if (sModel.AttributeRouteModel is null || sModel.AttributeRouteModel.Template is null)
      {
         return false;
      }

      string template = sModel.AttributeRouteModel.Template;

      if (
         template.Contains(SqlAuthConsts.URLROUTEPARAMSRV, StringComparison.InvariantCultureIgnoreCase)
         || template.Contains(SqlAuthConsts.URLROUTEPARAMUSR, StringComparison.InvariantCultureIgnoreCase)
         || template.Contains(SqlAuthConsts.URLROUTEPARAMDB, StringComparison.InvariantCultureIgnoreCase))
      {
         return false;
      }

      if (template.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
         ?.Equals(SqlAuthConsts.SQLAUTHAREA, StringComparison.InvariantCultureIgnoreCase) ?? false)
      {
         return false;
      }

      if (_templateBuilder.RootIsTopLevel)
      {
         // match any template
         return true;
      }

      if (_templateBuilder.RootIsTopLevelFolder
         && (template.Trim('/') == _templateBuilder.EffectiveRoot.Trim('/')
            || template.TrimStart('/').StartsWith(_templateBuilder.EffectiveRoot.TrimStart('/') + "/")))
      {
         return true;
      }

      return template.Trim('/').StartsWith(_templateBuilder.EffectiveRoot);
   }
}

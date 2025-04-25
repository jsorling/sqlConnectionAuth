using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.razor;

public class SqlAuthPageRouteModelConvention(SqlAuthAppPaths path) : IPageRouteModelConvention
{
   private readonly SqlAuthAppPaths _path = path;

   public void Apply(PageRouteModel model) {
      string p = _path.Root.TrimStart('/').TrimEnd('/');
      foreach (SelectorModel selector in model.Selectors)
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
}

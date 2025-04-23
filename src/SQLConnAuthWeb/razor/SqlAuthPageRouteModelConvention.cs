using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.razor;

public class SqlAuthPageRouteModelConvention(SqlAuthOptions options) : IPageRouteModelConvention
{
   private readonly SqlAuthOptions _options = options;

   public void Apply(PageRouteModel model) {
      string p = _options.SqlRootPath.TrimStart('/').TrimEnd('/');
      foreach (SelectorModel selector in model.Selectors)
      {
         string? newtemplate = null;
         if (selector.AttributeRouteModel!.Template!.StartsWith(p + "/")
            || selector.AttributeRouteModel.Template == p)
            newtemplate = $"/{p}/{{{SqlAuthConsts.URLROUTEPARAMSRV}}}/{{{SqlAuthConsts.URLROUTEPARAMUSR}}}"
               + (string.IsNullOrWhiteSpace(_options.SqlTailPath.Trim('/')) ? "/" : "/" + _options.SqlTailPath.Trim('/') + "/")
               + selector.AttributeRouteModel.Template[p.Length..].TrimStart('/');

         if (newtemplate is not null)
         {
            selector.AttributeRouteModel.Template = newtemplate;
         }
      }
   }
}

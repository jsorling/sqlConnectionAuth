﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Sorling.SqlConnAuthWeb.razor;

public class SqlConnAuthPageRouteModelConvention(string sqlRootPath = "db") : IPageRouteModelConvention
{
   public string SqlRootPath { get; init; } = sqlRootPath;

   public void Apply(PageRouteModel model) {
      string p = SqlRootPath.TrimStart('/').TrimEnd('/');
      foreach (SelectorModel selector in model.Selectors) {
         string? newtemplate = null;
         if (selector.AttributeRouteModel!.Template!.StartsWith(p + "/")
            || selector.AttributeRouteModel.Template == p)
            newtemplate = $"/{p}/{{{SqlConnAuthConsts.URLROUTEPARAMSRV}}}/{{{SqlConnAuthConsts.URLROUTEPARAMUSR}}}/srv/"
               + selector.AttributeRouteModel.Template[p.Length..].TrimStart('/');

         if (newtemplate is not null) {
            selector.AttributeRouteModel.Template = newtemplate;
         }
      }
   }
}

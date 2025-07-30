using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.helpers;

public class RouteTemplateBuilder(SqlAuthAppPaths paths)
{
   private readonly SqlAuthAppPaths _paths = paths ?? throw new ArgumentNullException(nameof(paths));

   public string EffectiveRoot => string.IsNullOrEmpty(_paths.Root.Trim()) ? "/" : '/' + _paths.Root.Trim('/') + '/';

   public string Tail => string.IsNullOrEmpty(_paths.Tail.Trim()) ? "/" : '/' + _paths.Tail.Trim('/') + '/';

   public string BasePath
      => $"{EffectiveRoot}{{{SqlAuthConsts.URLROUTEPARAMSRV}}}/{{{SqlAuthConsts.URLROUTEPARAMUSR}}}"
         + (_paths.UseDBNameRouting ? $"/{{{SqlAuthConsts.URLROUTEPARAMDB}}}" : string.Empty)
         + Tail;

   public string BuildTemplate(string template) 
      => BasePath + template.TrimStart('/');
}

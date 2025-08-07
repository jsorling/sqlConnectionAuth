using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.helpers;

public class RouteTemplateBuilder(SqlAuthAppPaths paths)
{
   private readonly SqlAuthAppPaths _paths = paths ?? throw new ArgumentNullException(nameof(paths));

   public string EffectiveRoot => string.Join("/", RootParts);

   public string[] RootParts => _paths.Root?.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];

   public bool RootIsTopLevel => RootParts.Length == 0;

   public bool RootIsTopLevelFolder => RootParts.Length == 1;

   public string Tail => string.IsNullOrEmpty(_paths.Tail) ? string.Empty : _paths.Tail.Trim('/') + '/';

   public string BaseTemplatePath
      => $"{EffectiveRoot}{(RootIsTopLevel ? "" : "/")}{{{SqlAuthConsts.URLROUTEPARAMSRV}}}/{{{SqlAuthConsts.URLROUTEPARAMUSR}}}/"
         + (_paths.UseDBNameRouting ? $"{{{SqlAuthConsts.URLROUTEPARAMDB}}}/" : string.Empty)
         + Tail;

   public string BuildTemplate(string template)
      => (template.StartsWith('/') ? "/" : string.Empty)
         + BaseTemplatePath
         + template.Trim('/')[EffectiveRoot.Length..].TrimStart('/');
}

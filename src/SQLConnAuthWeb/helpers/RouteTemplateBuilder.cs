using Sorling.SqlConnAuthWeb.authentication;

namespace Sorling.SqlConnAuthWeb.helpers;

/// <summary>
/// Provides methods for building route templates for SQL authentication endpoints based on application path configuration.
/// </summary>
/// <param name="paths">The application path configuration for SQL authentication.</param>
public class RouteTemplateBuilder(SqlAuthAppPaths paths)
{
   private readonly SqlAuthAppPaths _paths = paths ?? throw new ArgumentNullException(nameof(paths));

   /// <summary>
   /// Gets the effective root path as a single string, with segments joined by '/'.
   /// </summary>
   public string EffectiveRoot => string.Join("/", RootParts);

   /// <summary>
   /// Gets the root path segments as an array, split by '/'.
   /// </summary>
   public string[] RootParts => _paths.Root?.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];

   /// <summary>
   /// Gets a value indicating whether the root path is at the top level (no segments).
   /// </summary>
   public bool RootIsTopLevel => RootParts.Length == 0;

   /// <summary>
   /// Gets a value indicating whether the root path is a single top-level folder.
   /// </summary>
   public bool RootIsTopLevelFolder => RootParts.Length == 1;

   /// <summary>
   /// Gets the tail segment, trimmed and suffixed with a '/'. Returns an empty string if no tail is set.
   /// </summary>
   public string Tail => string.IsNullOrEmpty(_paths.Tail) ? string.Empty : _paths.Tail.Trim('/') + '/';

   /// <summary>
   /// Gets the base template path for SQL authentication routes, including root, server, user, and optionally database segments.
   /// </summary>
   public string BaseTemplatePath
      => $"{EffectiveRoot}{(RootIsTopLevel ? "" : "/")}{{{SqlAuthConsts.URLROUTEPARAMSRV}}}/{{{SqlAuthConsts.URLROUTEPARAMUSR}}}/"
         + (_paths.UseDBNameRouting ? $"{{{SqlAuthConsts.URLROUTEPARAMDB}}}/" : string.Empty)
         + Tail;

   /// <summary>
   /// Builds a complete route template by combining the base template path with the specified template.
   /// </summary>
   /// <param name="template">The template to append to the base path.</param>
   /// <returns>The full route template string.</returns>
   public string BuildTemplate(string template)
      => (template.StartsWith('/') ? "/" : string.Empty)
         + BaseTemplatePath
         + template.Trim('/')[EffectiveRoot.Length..].TrimStart('/');
}

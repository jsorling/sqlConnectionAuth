namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents application path configuration for SQL authentication, including root and tail segments.
/// </summary>
/// <param name="Root">The root segment of the path (default is "/db").</param>
/// <param name="Tail">The tail segment of the path (default is "srv").</param>
public record SqlAuthAppPaths(string Root = "/db", string Tail = "", bool UseDBNameRouting = true)
{
   /// <summary>
   /// Constructs a URI-escaped SQL path using the specified server and user names, along with the configured root and tail segments.
   /// </summary>
   /// <param name="server">The SQL server name. Must not be null or empty.</param>
   /// <param name="user">The user name. Must not be null or empty.</param>
   /// <returns>A URI-escaped path string for SQL authentication context.</returns>
   /// <exception cref="ArgumentException">Thrown if <paramref name="server"/> or <paramref name="user"/> is null or empty.</exception>
   public string UriEscapedSqlPath(string server, string user) {
      ArgumentException.ThrowIfNullOrEmpty(server, nameof(server));
      ArgumentException.ThrowIfNullOrEmpty(user, nameof(user));

      return $"/{Uri.EscapeDataString(Root.Trim('/'))}"
         + $"/{Uri.EscapeDataString(server)}"
         + $"/{Uri.EscapeDataString(user)}"
         + (string.IsNullOrWhiteSpace(Tail.Trim('/'))
               ? ""
               : "/" + Tail.Trim('/')
           );
   }
}


namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlAuthAppPaths(string Root = "/db", string Tail = "srv")
{
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


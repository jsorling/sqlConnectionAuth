namespace Sorling.SqlConnAuthWeb.authentication;

public class SqlAuthOptions
{
   public string SqlRootPath { get; set; } = "/db";

   public string SqlTailPath { get; set; } = "srv";

   public bool AllowIntegratedSecurity { get; set; }

   public string? ThemeSwitcherLocalStorageName { get; set; }

   public bool UseThemeSwitcher() => !string.IsNullOrEmpty(ThemeSwitcherLocalStorageName);

   public bool AllowTrustServerCertificate { get; set; }

   public bool AllowLoopbackConnections { get; set; }

   public bool AllowPrivateNetworkConnections { get; set; }

   public string UriEscapedSqlPath(string server, string user) {
      ArgumentException.ThrowIfNullOrEmpty(server, nameof(server));
      ArgumentException.ThrowIfNullOrEmpty(user, nameof(user));

      return $"/{Uri.EscapeDataString(SqlRootPath.Trim('/'))}"
         + $"/{Uri.EscapeDataString(server)}"
         + $"/{Uri.EscapeDataString(user)}"
         + (string.IsNullOrWhiteSpace(SqlTailPath.Trim('/'))
               ? ""
               : "/" + SqlTailPath.Trim('/')
           );
   }
}

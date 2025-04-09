using Microsoft.Data.SqlClient;

namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlConnAuthenticationData(string? SqlServer, string? UserName, SqlConnAuthStoredSecrets StoredSecrets)
{
   public bool IsWinAuth => UserName == SqlConnAuthConsts.WINDOWSAUTHENTICATION
      && StoredSecrets.Password == SqlConnAuthConsts.WINDOWSAUTHENTICATION;

   public string UriEscServer => Uri.EscapeDataString(SqlServer ?? "");

   public string UriEscUser => Uri.EscapeDataString(UserName ?? "");

   public string ConnectionString(string? db = null) =>
      IsWinAuth
         ? string.IsNullOrEmpty(db)
         ? new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            IntegratedSecurity = true,
            TrustServerCertificate = StoredSecrets.TrustServerCertificate
         }.ConnectionString
         : new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            InitialCatalog = db,
            IntegratedSecurity = true,
            TrustServerCertificate = StoredSecrets.TrustServerCertificate
         }.ConnectionString
         : string.IsNullOrEmpty(db)
         ? new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            Password = StoredSecrets.Password,
            UserID = UserName,
            TrustServerCertificate = StoredSecrets.TrustServerCertificate
         }.ConnectionString
         : new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            InitialCatalog = db,
            Password = StoredSecrets.Password,
            UserID = UserName,
            TrustServerCertificate = StoredSecrets.TrustServerCertificate
         }.ConnectionString;
}

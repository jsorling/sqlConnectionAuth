using Microsoft.Data.SqlClient;

namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlAuthConnectionstringProvider(string? SqlServer, string? UserName, SqlAuthStoredSecrets StoredSecrets)
{
   public bool IsWinAuth => UserName == SqlAuthConsts.WINDOWSAUTHENTICATION
      && StoredSecrets.Password == SqlAuthConsts.WINDOWSAUTHENTICATION;

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

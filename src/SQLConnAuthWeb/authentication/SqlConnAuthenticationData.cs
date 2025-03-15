using Microsoft.Data.SqlClient;

namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlConnAuthenticationData(string? SqlServer, string? UserName, string? Password)
{
   public bool IsWinAuth => UserName == SqlConnAuthConsts.WINDOWSAUTHENTICATION
      && Password == SqlConnAuthConsts.WINDOWSAUTHENTICATION;

   public string UriEscServer => Uri.EscapeDataString(SqlServer ?? "");

   public string UriEscUser => Uri.EscapeDataString(UserName ?? "");

   public string ConnectionString(string? db = null) =>
      IsWinAuth
         ? string.IsNullOrEmpty(db)
         ? new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            IntegratedSecurity = true
         }.ConnectionString
         : new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            InitialCatalog = db,
            IntegratedSecurity = true
         }.ConnectionString
         : string.IsNullOrEmpty(db)
         ? new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            Password = Password,
            UserID = UserName
         }.ConnectionString
         : new SqlConnectionStringBuilder() {
            DataSource = SqlServer,
            InitialCatalog = db,
            Password = Password,
            UserID = UserName
         }.ConnectionString;
}

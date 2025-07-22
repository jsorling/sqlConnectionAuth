using Microsoft.Data.SqlClient;

namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Provides logic for building SQL connection strings based on authentication context and stored secrets.
/// </summary>
/// <param name="SqlServer">The SQL Server name or address.</param>
/// <param name="UserName">The user name for authentication, or a special value for Windows Authentication.</param>
/// <param name="StoredSecrets">The stored secrets containing password and connection options.</param>
public record SqlAuthConnectionstringProvider(string? SqlServer, string? UserName, SqlAuthStoredSecrets StoredSecrets)
{
    /// <summary>
    /// Gets a value indicating whether Windows Authentication is used.
    /// </summary>
    public bool IsWinAuth => UserName == SqlAuthConsts.WINDOWSAUTHENTICATION
        && StoredSecrets.Password == SqlAuthConsts.WINDOWSAUTHENTICATION;

    /// <summary>
    /// Builds a SQL connection string for the specified database and authentication context.
    /// </summary>
    /// <param name="db">The database name, or null for the default database.</param>
    /// <returns>A SQL connection string configured for the current authentication context.</returns>
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

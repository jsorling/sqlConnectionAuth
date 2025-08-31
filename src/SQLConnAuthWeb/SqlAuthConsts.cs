namespace Sorling.SqlConnAuthWeb;

/// <summary>
/// Contains constants used for SQL Connection Authentication in the SqlConnAuthWeb project.
/// </summary>
public class SqlAuthConsts
{
   /// <summary>
   /// The authentication scheme name for SQL connection authentication.
   /// </summary>
   public const string SQLAUTHSCHEME = "SQLConnAuthScheme";

   /// <summary>
   /// The policy name for SQL connection authentication.
   /// </summary>
   public const string SQLAUTHPOLICY = "SQLConnAuthPolicy";

   /// <summary>
   /// The area name for SQL connection authentication routes.
   /// </summary>
   public const string SQLAUTHAREA = "sqlconnauth";

   /// <summary>
   /// Claim type for the SQL Server name.
   /// </summary>
   public const string CLAIMSQLSERVER = "SQLConnAuthSqlServer";

   /// <summary>
   /// Claim type for the SQL username.
   /// </summary>
   public const string CLAIMSQLUSERNAME = "SQLConnAuthUserName";

   /// <summary>
   /// Claim type for the SQL password reference.
   /// </summary>
   public const string CLAIMSQLPASSWORDREF = "SQLConnAuthPasswordRef";

   /// <summary>
   /// Claim type for the SQL password.
   /// </summary>
   public const string CLAIMSQLPASSWORD = "SQLConnAuthPassword";

   /// <summary>
   /// Route parameter for SQL server.
   /// </summary>
   public const string URLROUTEPARAMSRV = "sqlauthparamsrv";

   /// <summary>
   /// Route parameter for SQL user.
   /// </summary>
   public const string URLROUTEPARAMUSR = "sqlauthparamusr";

   /// <summary>
   /// Route parameter for SQL database name.
   /// </summary>
   public const string URLROUTEPARAMDB = "sqlauthparamdb";

   /// <summary>
   /// Route parameter for a temporary SQL password.
   /// </summary>
   public const string URLROUTEPARAMTEMPPWD = "sqlauthparamtemppwd";

   /// <summary>
   /// Value representing Windows Authentication.
   /// </summary>
   public const string WINDOWSAUTHENTICATION = "--win--auth--";

   /// <summary>
   /// The route template for selecting a SQL database, including server, user, and temporary password parameters.
   /// Example: <c>{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamtemppwd}</c>
   /// </summary>
   public const string URLROUTETEMPLATESELECTDB = $"{{{URLROUTEPARAMSRV}}}/{{{URLROUTEPARAMUSR}}}/{{{URLROUTEPARAMTEMPPWD}}}";

   /// <summary>
   /// The route template for connecting to a SQL database, including server, user, and optional database name parameters.
   /// Example: <c>{sqlauthparamsrv}/{sqlauthparamusr}/{sqlauthparamdb}?</c>
   /// </summary>
   public const string URLROUTETEMPLATECONNECT = $"{{{URLROUTEPARAMSRV}}}/{{{URLROUTEPARAMUSR}}}/{{{URLROUTEPARAMDB}?}}";

   /// <summary>
   /// Query parameter key for a temporary password key.
   /// </summary>
   public const string QUERYPARAMTMPPWDKEY = "sqlauthtmppwdkey";

   /// <summary>
   /// Placeholder value used to indicate the return URL after selecting a SQL database.
   /// </summary>
   public const string RETURNURLSELECTDBPLACEHOLDER = "--sqlauthselecteddb";
}

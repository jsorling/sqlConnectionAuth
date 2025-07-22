namespace Sorling.SqlConnAuthWeb.authentication;

/// <summary>
/// Represents the result of a SQL authentication attempt, including success status, exception details, and SQL Server version.
/// </summary>
/// <param name="Success">Indicates whether authentication was successful.</param>
/// <param name="Exception">The exception encountered during authentication, if any; otherwise, null.</param>
/// <param name="SqlVersion">The version of the SQL Server, if available; otherwise, null.</param>
public record SqlAuthenticationResult(bool Success, Exception? Exception, string? SqlVersion);

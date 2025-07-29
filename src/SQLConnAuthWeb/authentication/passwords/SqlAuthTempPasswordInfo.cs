namespace Sorling.SqlConnAuthWeb.authentication.passwords;

/// <summary>
/// Contains temporary password information for SQL authentication.
/// </summary>
/// <param name="Password">The temporary password value.</param>
/// <param name="TrustServerCertificate">Indicates whether to trust the server certificate.</param>
public record SqlAuthTempPasswordInfo(string Password, bool TrustServerCertificate);

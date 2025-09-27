namespace Sorling.SqlConnAuthWeb.authentication.passwords;

/// <summary>
/// Contains temporary password information for SQL authentication.
/// </summary>
/// <param name="Password">The temporary password value.</param>
/// <param name="TrustServerCertificate">Indicates whether to trust the server certificate.</param>
public record SqlAuthTempPasswordInfo(string Password, bool TrustServerCertificate)
{
   /// <summary>
   /// Implicitly converts a <see cref="SqlAuthTempPasswordInfo"/> to <see cref="SqlAuthStoredSecrets"/>.
   /// </summary>
   /// <param name="tmppwd">The temporary password info to convert.</param>
   /// <returns>
   /// A <see cref="SqlAuthStoredSecrets"/> instance with the password and trust flag copied from <paramref name="tmppwd"/>,
   /// and <c>RuleReValidationAfter</c> set to <c>null</c> (no scheduled revalidation).
   /// </returns>
   public static implicit operator SqlAuthStoredSecrets(SqlAuthTempPasswordInfo tmppwd)
      => new(tmppwd.Password, tmppwd.TrustServerCertificate, null);
}

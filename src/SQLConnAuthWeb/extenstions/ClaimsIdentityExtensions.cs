using Sorling.SqlConnAuthWeb.authentication;
using System.Security.Claims;

namespace Sorling.SqlConnAuthWeb.extenstions;

/// <summary>
/// Provides extension methods for working with <see cref="ClaimsIdentity"/> collections in the context of SQL authentication.
/// </summary>
internal static class ClaimsIdentityExtensions
{
    /// <summary>
    /// Extracts <see cref="SqlAuthCookieClaims"/> from a collection of <see cref="ClaimsIdentity"/> objects for the SQL authentication scheme.
    /// </summary>
    /// <param name="ci">The collection of claims identities.</param>
    /// <returns>The <see cref="SqlAuthCookieClaims"/> if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ci"/> is null.</exception>
    internal static SqlAuthCookieClaims? SqlConnAuthCookieClaims(this IEnumerable<ClaimsIdentity> ci) {
        ArgumentNullException.ThrowIfNull(ci);
        ClaimsIdentity? id = ci.FirstOrDefault(w => w.AuthenticationType == SqlAuthConsts.SQLAUTHSCHEME);

        return id is null ? null
            : new(id.Claims.Where(f => f.Type == SqlAuthConsts.CLAIMSQLSERVER).Select(s => s.Value).FirstOrDefault() ?? ""
            , id.Claims.Where(w => w.Type == SqlAuthConsts.CLAIMSQLUSERNAME).Select(s => s.Value).FirstOrDefault() ?? ""
            , id.Claims.Where(w => w.Type == SqlAuthConsts.CLAIMSQLPASSWORDREF).Select(s => s.Value).FirstOrDefault() ?? ""
            );
    }
}

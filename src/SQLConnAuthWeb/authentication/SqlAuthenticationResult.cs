namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlAuthenticationResult(bool Success, Exception? Exception, string? SqlVersion);

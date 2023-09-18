namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlConnAuthenticationResult(bool Success, Exception? Exception, string? SqlVersion);

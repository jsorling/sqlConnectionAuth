namespace Sorling.SqlConnAuthWeb.authentication;

public record SQLAuthenticateRequest(string Password, bool TrustServerCertificate = false);

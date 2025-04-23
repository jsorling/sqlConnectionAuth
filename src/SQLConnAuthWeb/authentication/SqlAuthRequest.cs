namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlAuthRequest(
    string Password,
    bool TrustServerCertificate
);

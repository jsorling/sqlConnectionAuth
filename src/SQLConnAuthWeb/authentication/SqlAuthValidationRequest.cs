namespace Sorling.SqlConnAuthWeb.authentication;

public record SqlAuthValidationRequest(string Datasource, string UserName, string Password, bool TrustServerCertificate = false);

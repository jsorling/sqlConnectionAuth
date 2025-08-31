namespace Sorling.SqlConnAuthWeb.authentication.dbaccess;

/// <summary>
/// Represents a SQL database with a name property for use in authentication scenarios.
/// </summary>
/// <param name="Name">The name of the SQL database.</param>
public record SqlDatabase(string Name) : ISqlDatabase;


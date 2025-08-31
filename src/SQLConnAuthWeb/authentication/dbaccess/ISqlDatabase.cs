namespace Sorling.SqlConnAuthWeb.authentication.dbaccess;

/// <summary>
/// Defines the contract for a SQL database entity with a name property.
/// </summary>
public interface ISqlDatabase
{
   /// <summary>
   /// Gets the name of the SQL database.
   /// </summary>
   string Name { get; }
}

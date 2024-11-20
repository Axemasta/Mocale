namespace Mocale.Cache.SQLite.Abstractions;

/// <summary>
/// SQLite Database Connection Provider
/// </summary>
public interface IDatabaseConnectionProvider
{
    /// <summary>
    /// Get connection to the sqlite database
    /// </summary>
    /// <returns></returns>
    SQLiteConnection GetDatabaseConnection();
}

namespace Mocale.Cache.SQLite.Abstractions;

/// <summary>
/// Database Path Provider
/// </summary>
public interface IDatabasePathProvider
{
    /// <summary>
    /// Get SQLite database path on the device
    /// </summary>
    /// <returns></returns>
    string GetDatabasePath();
}

namespace Mocale.Cache.SQLite.Abstractions;

/// <summary>
/// Configuration For SQLite Cache Library
/// </summary>
public interface ISqliteConfig
{
    /// <summary>
    /// The name of the cache database
    /// </summary>
    string DatabaseName { get; }

    /// <summary>
    /// The database directory: FileSystem.AppDataDirectory
    /// </summary>
    string DatabaseDirectory { get; }
}

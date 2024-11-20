namespace Mocale.Cache.SQLite.Models;

/// <inheritdoc/>
public class SqliteConfig : ISqliteConfig
{
    /// <inheritdoc/>
    public string DatabaseName { get; private set; } = Constants.DatabaseFileName;

    /// <inheritdoc/>
    public string DatabaseDirectory { get; internal set; } = string.Empty;

    /// <inheritdoc/>
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromDays(1);
}

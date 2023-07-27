namespace Mocale.Cache.SQLite.Models;

public class SqliteConfig : ISqliteConfig
{
    public string DatabaseName { get; private set; } = Constants.DatabaseFileName;

    public required string DatabaseDirectory { get; set; }

    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromDays(1);
}
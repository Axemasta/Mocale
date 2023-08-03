namespace Mocale.Cache.SQLite.Models;

public class SqliteConfig : ISqliteConfig
{
    public string DatabaseName { get; private set; } = Constants.DatabaseFileName;

    public string DatabaseDirectory { get; internal set; } = string.Empty;

    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromDays(1);
}

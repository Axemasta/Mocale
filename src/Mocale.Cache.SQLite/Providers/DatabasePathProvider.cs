using Mocale.Abstractions;
namespace Mocale.Cache.SQLite.Providers;

public class DatabasePathProvider : IDatabasePathProvider
{
    private readonly IConfigurationManager<ISqliteConfig> configurationManager;

    public DatabasePathProvider(IConfigurationManager<ISqliteConfig> configurationManager)
    {
        this.configurationManager = Guard.Against.Null(configurationManager, nameof(configurationManager));
    }

    public string GetDatabasePath()
    {
        var config = configurationManager.Configuration;

        return Path.Combine(config.DatabaseDirectory, config.DatabaseName);
    }
}

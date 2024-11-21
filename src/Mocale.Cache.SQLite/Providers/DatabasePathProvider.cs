using Mocale.Abstractions;
namespace Mocale.Cache.SQLite.Providers;

internal class DatabasePathProvider(IConfigurationManager<ISqliteConfig> configurationManager)
    : IDatabasePathProvider
{
    private readonly IConfigurationManager<ISqliteConfig> configurationManager = Guard.Against.Null(configurationManager, nameof(configurationManager));

    public string GetDatabasePath()
    {
        var config = configurationManager.Configuration;

        return Path.Combine(config.DatabaseDirectory, config.DatabaseName);
    }
}

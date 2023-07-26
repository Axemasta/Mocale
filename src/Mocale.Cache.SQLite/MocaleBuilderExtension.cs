using Mocale.Abstractions;
using Mocale.Cache.SQLite.Managers;
using Mocale.Cache.SQLite.Providers;
using Mocale.Cache.SQLite.Repositories;
using Mocale.Managers;
namespace Mocale.Cache.SQLite;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseSqliteCache(this MocaleBuilder builder)
    {
        // TODO: Does this even need config?

        var config = new SqliteConfig
        {
            DatabaseDirectory = FileSystem.AppDataDirectory,
        };

        var configurationManager = new ConfigurationManager<ISqliteConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<ISqliteConfig>>(configurationManager);

        builder.AppBuilder.Services.AddTransient<ICacheUpdateManager, SqlCacheUpdateManager>();
        builder.AppBuilder.Services.AddSingleton<IDatabaseConnectionProvider, DatabaseConnectionProvider>();
        builder.AppBuilder.Services.AddTransient<IDatabasePathProvider, DatabasePathProvider>();
        builder.AppBuilder.Services.AddSingleton<ILocalisationCacheManager, LocalisationCacheManager>();
        builder.AppBuilder.Services.AddSingleton<ICacheRepository, CacheRepository>();
        builder.AppBuilder.Services.AddSingleton<ITranslationsRepository, TranslationsRepository>();

        builder.CacheProviderRegistered = true;

        return builder;
    }
}

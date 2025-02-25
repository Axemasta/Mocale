using Mocale.Abstractions;
using Mocale.Cache.SQLite.Managers;
using Mocale.Cache.SQLite.Providers;
using Mocale.Cache.SQLite.Repositories;
using Mocale.Managers;
namespace Mocale.Cache.SQLite;

/// <summary>
/// Mocale Builder Extension
/// </summary>
public static class MocaleBuilderExtension
{
    /// <summary>
    /// Use SQLite to cache localizations acquired by external providers
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureSql"></param>
    /// <returns></returns>
    public static MocaleBuilder UseSqliteCache(this MocaleBuilder builder, Action<SqliteConfig> configureSql)
    {
        return UseSqliteCache(builder, FileSystem.Current, configureSql);
    }

    internal static MocaleBuilder UseSqliteCache(this MocaleBuilder builder, IFileSystem fileSystem, Action<SqliteConfig> configureSql)
    {
        var config = new SqliteConfig
        {
            DatabaseDirectory = fileSystem.AppDataDirectory,
        };

        configureSql(config);

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

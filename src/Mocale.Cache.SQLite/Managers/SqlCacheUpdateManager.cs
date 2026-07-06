using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;

namespace Mocale.Cache.SQLite.Managers;

internal partial class SqlCacheUpdateManager : ICacheUpdateManager
{
    private readonly ICacheRepository cacheRepository;
    private readonly ILogger logger;

    private readonly ISqliteConfig sqliteConfig;
    private readonly TimeProvider timeProvider;

    public SqlCacheUpdateManager(
        ICacheRepository cacheRepository,
        ILogger<SqlCacheUpdateManager> logger,
        IConfigurationManager<ISqliteConfig> sqliteConfigurationManager,
        TimeProvider timeProvider)
    {
        this.cacheRepository = Guard.Against.Null(cacheRepository);
        this.logger = Guard.Against.Null(logger);
        this.timeProvider = Guard.Against.Null(timeProvider);

        sqliteConfigurationManager = Guard.Against.Null(sqliteConfigurationManager);
        sqliteConfig = sqliteConfigurationManager.Configuration;
    }

    #region Interface Implementations

    /// <inheritdoc/>
    public bool CanUpdateCache(CultureInfo cultureInfo)
    {
        var updateItem = cacheRepository.GetItem(cultureInfo);

        if (updateItem is null)
        {
            return true;
        }

        var nextUpdateWindow = updateItem.LastUpdated.Add(sqliteConfig.UpdateInterval);

        return nextUpdateWindow < timeProvider.GetUtcNow();
    }

    /// <inheritdoc/>
    public bool SetCacheUpdated(CultureInfo cultureInfo)
    {
        return cacheRepository.AddOrUpdateItem(cultureInfo, timeProvider.GetUtcNow().DateTime);
    }

    /// <inheritdoc/>
    public void ClearCache(CultureInfo cultureInfo)
    {
        var deleted = cacheRepository.DeleteItem(cultureInfo);

        if (!deleted)
        {
            LogUnableToDeleteCacheForCultureCultureName(cultureInfo.Name);
            return;
        }

        LogDeletedUpdateCacheForCultureCultureName(cultureInfo.Name);
    }

    /// <inheritdoc/>
    public void ClearCache()
    {
        var deleted = cacheRepository.DeleteAll();

        if (!deleted)
        {
            LogUnableToDeleteCacheForAllCultures();
            return;
        }

        LogDeletedUpdateCacheForAllCultures();
    }

    #endregion Interface Implementations

    #region Logging

    [LoggerMessage(LogLevel.Trace, "Deleted update cache for culture: {CultureName}")]
    partial void LogDeletedUpdateCacheForCultureCultureName(string cultureName);

    [LoggerMessage(LogLevel.Warning, "Unable to delete cache for culture: {CultureName}")]
    partial void LogUnableToDeleteCacheForCultureCultureName(string cultureName);

    [LoggerMessage(LogLevel.Warning, "Unable to delete cache for all cultures")]
    partial void LogUnableToDeleteCacheForAllCultures();

    [LoggerMessage(LogLevel.Trace, "Deleted update cache for all cultures")]
    partial void LogDeletedUpdateCacheForAllCultures();

    #endregion Logging
}

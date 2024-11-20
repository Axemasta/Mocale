using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;

namespace Mocale.Cache.SQLite.Managers;

internal class SqlCacheUpdateManager : ICacheUpdateManager
{
    private readonly ICacheRepository cacheRepository;
    private readonly IDateTime dateTime;
    private readonly ILogger logger;

    private readonly ISqliteConfig sqliteConfig;

    public SqlCacheUpdateManager(
        ICacheRepository cacheRepository,
        IDateTime dateTime,
        ILogger<SqlCacheUpdateManager> logger,
        IConfigurationManager<ISqliteConfig> sqliteConfigurationManager)
    {
        this.dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.cacheRepository = Guard.Against.Null(cacheRepository, nameof(cacheRepository));

        sqliteConfigurationManager = Guard.Against.Null(sqliteConfigurationManager, nameof(sqliteConfigurationManager));
        this.sqliteConfig = sqliteConfigurationManager.Configuration;
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

        return nextUpdateWindow < dateTime.UtcNow;
    }

    /// <inheritdoc/>
    public bool SetCacheUpdated(CultureInfo cultureInfo)
    {
        return cacheRepository.AddOrUpdateItem(cultureInfo, dateTime.UtcNow);
    }

    /// <inheritdoc/>
    public void ClearCache(CultureInfo cultureInfo)
    {
        var deleted = cacheRepository.DeleteItem(cultureInfo);

        if (!deleted)
        {
            logger.LogWarning("Unable to delete cache for culture: {CultureName}", cultureInfo.Name);
            return;
        }

        logger.LogTrace("Deleted update cache for culture: {CultureName}", cultureInfo.Name);
    }

    /// <inheritdoc/>
    public void ClearCache()
    {
        var deleted = cacheRepository.DeleteAll();

        if (!deleted)
        {
            logger.LogWarning("Unable to delete cache for all cultures");
            return;
        }

        logger.LogTrace("Deleted update cache for all cultures");
    }

    #endregion Interface Implementations
}

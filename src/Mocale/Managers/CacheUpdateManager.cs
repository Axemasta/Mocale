using System.Globalization;
using Ardalis.GuardClauses;
using Mocale.DAL.Abstractions;

namespace Mocale.Managers;

public class CacheUpdateManager : ICacheUpdateManager
{
    private readonly ICacheRepository cacheRepository;
    private readonly IDateTime dateTime;
    private readonly ILogger logger;

    public CacheUpdateManager(
        ICacheRepository cacheRepository,
        IDateTime dateTime,
        ILogger<CacheUpdateManager> logger)
    {
        this.dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.cacheRepository = Guard.Against.Null(cacheRepository, nameof(cacheRepository));

        Lol();
    }

    #region Methods

    private static void Lol()
    {

    }

    #endregion Methods

    #region Interface Implementations

    public bool CanUpdateCache(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }

    public bool SetCacheUpdated(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }

    public void ClearCache(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }

    public void ClearCache()
    {
        throw new NotImplementedException();
    }

    #endregion Interface Implementations
}

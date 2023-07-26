using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
namespace Mocale.Cache.SQLite.Managers;

public class LocalisationCacheManager : ILocalisationCacheManager
{
    #region Fields

    private readonly ICacheRepository cacheRepository;
    private readonly ILogger logger;
    private readonly ITranslationsRepository translationsRepository;

    #endregion Fields

    #region Constructors

    public LocalisationCacheManager(
        ICacheRepository cacheRepository,
        ILogger<LocalisationCacheManager> logger,
        ITranslationsRepository translationsRepository)
    {
        this.cacheRepository = Guard.Against.Null(cacheRepository, nameof(cacheRepository));
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.translationsRepository = Guard.Against.Null(translationsRepository, nameof(translationsRepository));
    }

    #endregion Constructors

    #region Interface Implementations

    public Dictionary<string, string>? GetCachedLocalizations(CultureInfo cultureInfo)
    {
        return translationsRepository.GetTranslations(cultureInfo);
    }

    #endregion Interface Implementations
}

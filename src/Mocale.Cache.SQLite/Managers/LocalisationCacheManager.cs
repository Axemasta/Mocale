using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
namespace Mocale.Cache.SQLite.Managers;

public class LocalisationCacheManager : ILocalisationCacheManager
{
    #region Fields

    private readonly ICacheUpdateManager cacheUpdateManager;
    private readonly ILogger logger;
    private readonly ITranslationsRepository translationsRepository;

    #endregion Fields

    #region Constructors

    public LocalisationCacheManager(
        ICacheUpdateManager cacheUpdateManager,
        ILogger<LocalisationCacheManager> logger,
        ITranslationsRepository translationsRepository)
    {
        this.cacheUpdateManager = Guard.Against.Null(cacheUpdateManager, nameof(cacheUpdateManager));
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.translationsRepository = Guard.Against.Null(translationsRepository, nameof(translationsRepository));
    }

    #endregion Constructors

    #region Interface Implementations

    public Dictionary<string, string>? GetCachedLocalizations(CultureInfo cultureInfo)
    {
        return translationsRepository.GetTranslations(cultureInfo);
    }

    public bool SaveCachedLocalizations(CultureInfo cultureInfo, Dictionary<string, string> localizations)
    {
        var saved = translationsRepository.AddTranslations(cultureInfo, localizations);

        if (!saved)
        {
            logger.LogWarning("Failed to add translations for culture: {CultureName}", cultureInfo.Name);
            return false;
        }

        var cacheUpdated = cacheUpdateManager.SetCacheUpdated(cultureInfo);

        if (!cacheUpdated)
        {
            logger.LogWarning("Translations for culture: {CultureName} were saved to the cache database but the cache history was not updated", cultureInfo.Name);
        }

        return cacheUpdated;
    }

    #endregion Interface Implementations
}

using System.Globalization;
using Ardalis.GuardClauses;
namespace Mocale.Managers;

public class TranslationResolver : ITranslationResolver
{
    #region Fields

    private readonly ICacheUpdateManager cacheUpdateManager;
    private readonly IExternalLocalizationProvider externalLocalizationProvider;
    private readonly IInternalLocalizationProvider internalLocalizationProvider;
    private readonly ILocalisationCacheManager localisationCacheManager;
    private readonly ILogger logger;

    #endregion Fields

    #region Constructors

    public TranslationResolver(
        ICacheUpdateManager cacheUpdateManager,
        IExternalLocalizationProvider externalLocalizationProvider,
        IInternalLocalizationProvider internalLocalizationProvider,
        ILocalisationCacheManager localisationCacheManager,
        ILogger<TranslationResolver> logger)
    {
        this.cacheUpdateManager = Guard.Against.Null(cacheUpdateManager, nameof(cacheUpdateManager));
        this.externalLocalizationProvider = Guard.Against.Null(externalLocalizationProvider, nameof(externalLocalizationProvider));
        this.internalLocalizationProvider = Guard.Against.Null(internalLocalizationProvider, nameof(internalLocalizationProvider));
        this.localisationCacheManager = Guard.Against.Null(localisationCacheManager, nameof(localisationCacheManager));
        this.logger = Guard.Against.Null(logger, nameof(logger));
    }

    #endregion Constructors

    #region Interface Implementations

    /// <inheritdoc/>
    public Task<TranslationLoadResult> LoadTranslations(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }

    public TranslationLoadResult LoadLocalTranslations(CultureInfo cultureInfo)
    {
        var internalTranslations = internalLocalizationProvider.GetValuesForCulture(cultureInfo);

        var cachedTranslations = localisationCacheManager.GetCachedLocalizations(cultureInfo);

        if (internalTranslations is not null && cachedTranslations is null)
        {
            return new TranslationLoadResult()
            {
                Loaded = true,
                Source = TranslationSource.Internal,
                Translations = internalTranslations,
            };
        }

        if (internalTranslations is null || cachedTranslations is null)
        {
            return new TranslationLoadResult()
            {
                Loaded = false,
                Source = TranslationSource.Internal,
            };
        }

        var addedKeys = new List<string>();

        // Insert fallback values incase they don't exist
        foreach (var translation in internalTranslations)
        {
            var added = cachedTranslations.TryAdd(translation.Key, translation.Value);

            if (added)
            {
                addedKeys.Add(translation.Key);
            }
        }

        if (addedKeys.Any())
        {
            logger.LogInformation("The following keys were present in the local translations but not in the cache: {AddedKeys}", addedKeys);
        }

        var cacheTemperature = cacheUpdateManager.CanUpdateCache(cultureInfo)
            ? TranslationSource.ColdCache
            : TranslationSource.WarmCache;

        return new TranslationLoadResult()
        {
            Loaded = true,
            Source = cacheTemperature,
            Translations = cachedTranslations,
        };
    }

    #endregion
}

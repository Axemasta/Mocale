using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Managers;

internal partial class TranslationResolver(
    ICacheUpdateManager cacheUpdateManager,
    IExternalLocalizationProvider externalLocalizationProvider,
    IInternalLocalizationProvider internalLocalizationProvider,
    ILocalisationCacheManager localisationCacheManager,
    ILogger<TranslationResolver> logger)
    : ITranslationResolver
{
    #region Fields

    private readonly ICacheUpdateManager cacheUpdateManager = Guard.Against.Null(cacheUpdateManager);
    private readonly IExternalLocalizationProvider externalLocalizationProvider = Guard.Against.Null(externalLocalizationProvider);
    private readonly IInternalLocalizationProvider internalLocalizationProvider = Guard.Against.Null(internalLocalizationProvider);
    private readonly ILocalisationCacheManager localisationCacheManager = Guard.Against.Null(localisationCacheManager);
    private readonly ILogger logger = Guard.Against.Null(logger);

    #endregion Fields

    #region Methods

    private async Task<TranslationLoadResult> LoadTranslationsInternal(CultureInfo cultureInfo)
    {
        if (!cacheUpdateManager.CanUpdateCache(cultureInfo))
        {
            // We have up to date local copy of cache
            var cacheTranslations = localisationCacheManager.GetCachedLocalizations(cultureInfo);

            if (cacheTranslations is not null)
            {
                return new TranslationLoadResult { Loaded = true, Source = TranslationSource.WarmCache, Localization = new Localization(cultureInfo) { Translations = cacheTranslations } };
            }
        }

        LogUpdatingTranslationsForCultureCultureNameFromExternalProvider(cultureInfo.Name);

        var externalResult = await externalLocalizationProvider.GetValuesForCultureAsync(cultureInfo);

        if (!externalResult.Success || externalResult.Localizations is null)
        {
            LogNoExternalTranslationsWereLoadedForCultureCultureName(cultureInfo.Name);

            return new TranslationLoadResult { Loaded = false, Source = TranslationSource.External, Localization = Localization.Invariant };
        }

        var saved = localisationCacheManager.SaveCachedLocalizations(cultureInfo, externalResult.Localizations);

        if (!saved)
        {
            LogTranslationsWereUpdatedForCultureCultureNameHoweverTheyWereNotSavedToCache(cultureInfo.Name);
        }

        return new TranslationLoadResult { Loaded = true, Source = TranslationSource.External, Localization = new Localization(cultureInfo) { Translations = externalResult.Localizations } };
    }

    #endregion Method

    #region Interface Implementations

    /// <inheritdoc />
    public async Task<TranslationLoadResult> LoadTranslations(CultureInfo cultureInfo)
    {
        try
        {
            return await LoadTranslationsInternal(cultureInfo);
        }
        catch (Exception ex)
        {
            LogAnExceptionOccurredLoadingTranslationsForCultureCultureName(ex, cultureInfo.Name);

            return new TranslationLoadResult { Loaded = false, Source = TranslationSource.External, Localization = Localization.Invariant };
        }
    }

    public TranslationLoadResult LoadLocalTranslations(CultureInfo cultureInfo)
    {
        var internalTranslations = internalLocalizationProvider.GetValuesForCulture(cultureInfo);

        var cachedTranslations = localisationCacheManager.GetCachedLocalizations(cultureInfo);

        if (internalTranslations is not null && cachedTranslations is null)
        {
            return new TranslationLoadResult { Loaded = true, Source = TranslationSource.Internal, Localization = new Localization(cultureInfo) { Translations = internalTranslations } };
        }

        if (internalTranslations is null && cachedTranslations is not null)
        {
            return new TranslationLoadResult
            {
                Loaded = true,
                Source = cacheUpdateManager.CanUpdateCache(cultureInfo)
                    ? TranslationSource.ColdCache
                    : TranslationSource.WarmCache,
                Localization = new Localization(cultureInfo) { Translations = cachedTranslations }
            };
        }

        if (internalTranslations is null || cachedTranslations is null)
        {
            return new TranslationLoadResult { Loaded = false, Source = TranslationSource.Internal, Localization = Localization.Invariant };
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

        if (addedKeys.Count > 0)
        {
            LogTheFollowingKeysWerePresentInTheLocalTranslationsButNotInTheCacheAddedKeys(addedKeys);
        }

        var cacheTemperature = cacheUpdateManager.CanUpdateCache(cultureInfo)
            ? TranslationSource.ColdCache
            : TranslationSource.WarmCache;

        return new TranslationLoadResult { Loaded = true, Source = cacheTemperature, Localization = new Localization(cultureInfo) { Translations = cachedTranslations } };
    }

    #endregion

    #region Logging

    [LoggerMessage(LogLevel.Debug, "Updating translations for culture: {CultureName} from external provider")]
    partial void LogUpdatingTranslationsForCultureCultureNameFromExternalProvider(string cultureName);

    [LoggerMessage(LogLevel.Warning, "No external translations were loaded for culture: {CultureName}")]
    partial void LogNoExternalTranslationsWereLoadedForCultureCultureName(string cultureName);

    [LoggerMessage(LogLevel.Warning, "Translations were updated for culture: {CultureName}, however they were not saved to cache")]
    partial void LogTranslationsWereUpdatedForCultureCultureNameHoweverTheyWereNotSavedToCache(string cultureName);

    [LoggerMessage(LogLevel.Error, "An exception occurred loading translations for culture: {CultureName}")]
    partial void LogAnExceptionOccurredLoadingTranslationsForCultureCultureName(Exception exception, string cultureName);

    [LoggerMessage(LogLevel.Information, "The following keys were present in the local translations but not in the cache: {AddedKeys}")]
    partial void LogTheFollowingKeysWerePresentInTheLocalTranslationsButNotInTheCacheAddedKeys(List<string> addedKeys);

    #endregion Logging
}

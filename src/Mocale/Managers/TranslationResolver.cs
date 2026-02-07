using System.Globalization;
namespace Mocale.Managers;

internal class TranslationResolver(
    ICacheUpdateManager cacheUpdateManager,
    IExternalLocalizationProvider externalLocalizationProvider,
    IInternalLocalizationProvider internalLocalizationProvider,
    ILocalisationCacheManager localisationCacheManager,
    ILogger<TranslationResolver> logger,
    IETagCacheManager? etagCacheManager = null)
    : ITranslationResolver
{
    #region Methods

    private async Task<TranslationLoadResult> LoadTranslationsInternal(CultureInfo cultureInfo)
    {
        // Check if provider supports ETag caching
        if (externalLocalizationProvider is IETagSupport etagProvider && etagCacheManager is not null)
        {
            return await LoadTranslationsWithETagAsync(cultureInfo, etagProvider);
        }

        // Fall back to standard behavior without ETag support
        return await LoadTranslationsStandardAsync(cultureInfo);
    }

    private async Task<TranslationLoadResult> LoadTranslationsWithETagAsync(
        CultureInfo cultureInfo,
        IETagSupport etagProvider)
    {
        // Get the stored ETag for this culture
        var storedETag = etagCacheManager!.GetETag(cultureInfo);

        logger.LogDebug(
            "Requesting translations for culture {CultureName} with ETag: {ETag}",
            cultureInfo.Name,
            storedETag ?? "(none)");

        var etagResult = await etagProvider.GetValuesForCultureWithETagAsync(cultureInfo, storedETag);

        if (!etagResult.Success)
        {
            logger.LogWarning(
                "ETag request failed for culture {CultureName}, falling back to cache",
                cultureInfo.Name);

            // Try to use cached translations
            var cachedTranslations = localisationCacheManager.GetCachedLocalizations(cultureInfo);
            if (cachedTranslations is not null)
            {
                return new TranslationLoadResult
                {
                    Loaded = true,
                    Source = TranslationSource.ColdCache,
                    Localization = new Localization(cultureInfo)
                    {
                        Translations = cachedTranslations,
                    },
                };
            }

            return new TranslationLoadResult
            {
                Loaded = false,
                Source = TranslationSource.External,
                Localization = Localization.Invariant,
            };
        }

        // Resource was not modified - use cached version
        if (!etagResult.WasModified)
        {
            logger.LogDebug(
                "Resource not modified for culture {CultureName}, using cached translations",
                cultureInfo.Name);

            var cachedTranslations = localisationCacheManager.GetCachedLocalizations(cultureInfo);
            if (cachedTranslations is not null)
            {
                return new TranslationLoadResult
                {
                    Loaded = true,
                    Source = TranslationSource.WarmCache,
                    Localization = new Localization(cultureInfo)
                    {
                        Translations = cachedTranslations,
                    },
                };
            }

            // This shouldn't happen - we had an ETag but no cache
            logger.LogWarning(
                "Resource was not modified but no cached translations found for culture {CultureName}",
                cultureInfo.Name);
        }

        // Resource was modified or this is the first request
        if (etagResult.Localizations is null)
        {
            logger.LogWarning(
                "No translations returned from ETag request for culture {CultureName}",
                cultureInfo.Name);

            return new TranslationLoadResult
            {
                Loaded = false,
                Source = TranslationSource.External,
                Localization = Localization.Invariant,
            };
        }

        // Save the new translations and ETag
        var saved = localisationCacheManager.SaveCachedLocalizations(cultureInfo, etagResult.Localizations);
        if (!saved)
        {
            logger.LogWarning(
                "Translations were updated for culture {CultureName}, but were not saved to cache",
                cultureInfo.Name);
        }

        if (!string.IsNullOrEmpty(etagResult.ETag))
        {
            var etagSaved = etagCacheManager.SaveETag(cultureInfo, etagResult.ETag);
            if (!etagSaved)
            {
                logger.LogWarning(
                    "ETag was not saved for culture {CultureName}",
                    cultureInfo.Name);
            }
        }

        return new TranslationLoadResult
        {
            Loaded = true,
            Source = TranslationSource.External,
            Localization = new Localization(cultureInfo)
            {
                Translations = etagResult.Localizations,
            },
        };
    }

    private async Task<TranslationLoadResult> LoadTranslationsStandardAsync(CultureInfo cultureInfo)
    {
        if (!cacheUpdateManager.CanUpdateCache(cultureInfo))
        {
            // We have up to date local copy of cache
            var cacheTranslations = localisationCacheManager.GetCachedLocalizations(cultureInfo);

            if (cacheTranslations is not null)
            {
                return new TranslationLoadResult
                {
                    Loaded = true,
                    Source = TranslationSource.WarmCache,
                    Localization = new Localization(cultureInfo)
                    {
                        Translations = cacheTranslations,
                    },
                };
            }
        }

        logger.LogDebug("Updating translations for culture: {CultureName} from external provider", cultureInfo.Name);

        var externalResult = await externalLocalizationProvider.GetValuesForCultureAsync(cultureInfo);

        if (!externalResult.Success || externalResult.Localizations is null)
        {
            logger.LogWarning("No external translations were loaded for culture: {CultureName}", cultureInfo.Name);

            return new TranslationLoadResult
            {
                Loaded = false,
                Source = TranslationSource.External,
                Localization = Localization.Invariant,
            };
        }

        var saved = localisationCacheManager.SaveCachedLocalizations(cultureInfo, externalResult.Localizations);

        if (!saved)
        {
            logger.LogWarning("Translations were updated for culture: {CultureName}, however they were not saved to cache", cultureInfo.Name);
        }

        return new TranslationLoadResult
        {
            Loaded = true,
            Source = TranslationSource.External,
            Localization = new Localization(cultureInfo)
            {
                Translations = externalResult.Localizations,
            },
        };
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
            logger.LogError(ex, "An exception occurred loading translations for culture: {CultureName}", cultureInfo.Name);

            return new TranslationLoadResult
            {
                Loaded = false,
                Source = TranslationSource.External,
                Localization = Localization.Invariant,
            };
        }
    }

    public TranslationLoadResult LoadLocalTranslations(CultureInfo cultureInfo)
    {
        var internalTranslations = internalLocalizationProvider.GetValuesForCulture(cultureInfo);

        var cachedTranslations = localisationCacheManager.GetCachedLocalizations(cultureInfo);

        if (internalTranslations is not null && cachedTranslations is null)
        {
            return new TranslationLoadResult
            {
                Loaded = true,
                Source = TranslationSource.Internal,
                Localization = new Localization(cultureInfo)
                {
                    Translations = internalTranslations,
                },
            };
        }

        if (internalTranslations is null && cachedTranslations is not null)
        {
            return new TranslationLoadResult
            {
                Loaded = true,
                Source = cacheUpdateManager.CanUpdateCache(cultureInfo)
                    ? TranslationSource.ColdCache
                    : TranslationSource.WarmCache,
                Localization = new Localization(cultureInfo)
                {
                    Translations = cachedTranslations,
                },
            };
        }

        if (internalTranslations is null || cachedTranslations is null)
        {
            return new TranslationLoadResult
            {
                Loaded = false,
                Source = TranslationSource.Internal,
                Localization = Localization.Invariant,
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

        if (addedKeys.Count > 0)
        {
            logger.LogInformation("The following keys were present in the local translations but not in the cache: {AddedKeys}", addedKeys);
        }

        var cacheTemperature = cacheUpdateManager.CanUpdateCache(cultureInfo)
            ? TranslationSource.ColdCache
            : TranslationSource.WarmCache;

        return new TranslationLoadResult
        {
            Loaded = true,
            Source = cacheTemperature,
            Localization = new Localization(cultureInfo)
            {
                Translations = cachedTranslations,
            },
        };
    }

    #endregion
}

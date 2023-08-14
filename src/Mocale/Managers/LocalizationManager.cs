using System.Globalization;
using Ardalis.GuardClauses;
namespace Mocale.Managers;

public class LocalizationManager : ILocalizationManager
{
    private readonly ICurrentCultureManager currentCultureManager;
    private readonly ILogger logger;
    private readonly ITranslationResolver translationResolver;
    private readonly ITranslationUpdater translationUpdater;

    public CultureInfo CurrentCulture { get; private set; }

    public LocalizationManager(
        ICurrentCultureManager currentCultureManager,
        ILogger<LocalizationManager> logger,
        ITranslationResolver translationResolver,
        ITranslationUpdater translationUpdater)
    {
        this.currentCultureManager = Guard.Against.Null(currentCultureManager, nameof(currentCultureManager));
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.translationResolver = Guard.Against.Null(translationResolver, nameof(translationResolver));
        this.translationUpdater = Guard.Against.Null(translationUpdater, nameof(this.translationUpdater));

        CurrentCulture = currentCultureManager.GetActiveCulture();
    }

    public async Task<bool> SetCultureAsync(CultureInfo culture)
    {
        try
        {
            var result = await translationResolver.LoadTranslations(culture);

            if (!result.Loaded)
            {
                logger.LogWarning("Unable to load culture {CultureName}, no localizations found", culture.Name);
                return false;
            }

            var localTranslations = translationResolver.LoadLocalTranslations(culture);

            if (localTranslations.Loaded)
            {
                translationUpdater.UpdateTranslations(localTranslations.Localization, localTranslations.Source);
            }

            translationUpdater.UpdateTranslations(result.Localization, result.Source);

            currentCultureManager.SetActiveCulture(culture);

            logger.LogDebug("Updated localization culture to {CultureName}", culture.Name);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred loading culture: {CultureName}", culture.Name);

            return false;
        }
    }

    public async Task<bool> Initialize()
    {
        try
        {
            return await InitializeInternal();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred initializing LocalizationManager");
            return false;
        }
    }

    private Task<bool> InitializeInternal()
    {
        var localTranslations = translationResolver.LoadLocalTranslations(CurrentCulture);

        if (!localTranslations.Loaded)
        {
            logger.LogWarning("Unable to load translations for culture: {CultureName}", CurrentCulture.Name);
            return Task.FromResult(false);
        }

        translationUpdater.UpdateTranslations(localTranslations.Localization, localTranslations.Source);

        logger.LogTrace("Loaded local translations from source: {TranslationSource}", localTranslations.Source);

        if (localTranslations.Source is TranslationSource.Internal or TranslationSource.ColdCache)
        {
            logger.LogInformation("External translations can be updated, checking for newer copy...");

            // Check cache and go get up to date translations
            Task.Run(() => CheckForTranslationUpdates(CurrentCulture))
                .Forget();
        }

        return Task.FromResult(true);
    }

    private async Task CheckForTranslationUpdates(CultureInfo cultureInfo)
    {
        var external = await translationResolver.LoadTranslations(cultureInfo);

        if (!external.Loaded)
        {
            logger.LogWarning("Unable to load external translations for culture: {CultureInfo}", cultureInfo);
            return;
        }

        translationUpdater.UpdateTranslations(external.Localization, TranslationSource.External);
    }
}

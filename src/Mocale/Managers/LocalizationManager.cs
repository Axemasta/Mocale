using System.Globalization;
using Ardalis.GuardClauses;
namespace Mocale.Managers;

public class LocalizationManager : ILocalizationManager
{
    private readonly IMocaleConfiguration mocaleConfiguration;
    private readonly ILogger logger;
    private readonly IPreferences preferences;
    private readonly ITranslationResolver translationResolver;
    private readonly ITranslationUpdater translationUpdater;

    public CultureInfo CurrentCulture { get; private set; }

    public LocalizationManager(
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        ILogger<LocalizationManager> logger,
        IPreferences preferences,
        ITranslationResolver translationResolver,
        ITranslationUpdater translationUpdater)
    {
        mocaleConfigurationManager = Guard.Against.Null(mocaleConfigurationManager, nameof(mocaleConfigurationManager));

        mocaleConfiguration = mocaleConfigurationManager.Configuration;
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.preferences = Guard.Against.Null(preferences, nameof(preferences));
        this.translationResolver = Guard.Against.Null(translationResolver, nameof(translationResolver));
        this.translationUpdater = Guard.Against.Null(translationUpdater, nameof(this.translationUpdater));

        CurrentCulture = GetActiveCulture();
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

            CurrentCulture = culture;

            translationUpdater.UpdateTranslations(result.Localization, result.Source);

            SetActiveCulture(culture);

            logger.LogDebug("Updated localization culture to {CultureName}", culture.Name);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred loading culture: {CultureName}", culture.Name);

            return false;
        }
    }

    public async Task Initialize()
    {
        try
        {
            await InitializeInternal();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred initializing LocalizationManager");
        }
    }

    private CultureInfo GetActiveCulture()
    {
        var defaultCulture = mocaleConfiguration.DefaultCulture;

        if (!mocaleConfiguration.SaveCultureChanged)
        {
            return defaultCulture;
        }

        // Load Here
        var lastUsedCulture = preferences.Get(Constants.LastUsedCultureKey, string.Empty);

        if (string.IsNullOrEmpty(lastUsedCulture))
        {
            logger.LogTrace("Setting Last Used Culture as: {DefaultCulture}", defaultCulture);
            SetActiveCulture(defaultCulture);
            return defaultCulture;
        }

        if (lastUsedCulture.TryParseCultureInfo(out var cultureInfo))
        {
            return cultureInfo;
        }

        logger.LogWarning("Unable to parse culture from preferences: {LastUsedCulture}", lastUsedCulture);
        return defaultCulture;
    }

    private void SetActiveCulture(CultureInfo cultureInfo)
    {
        var cultureString = cultureInfo.ToString();

        preferences.Set(Constants.LastUsedCultureKey, cultureString);
    }

    private Task InitializeInternal()
    {
        var localTranslations = translationResolver.LoadLocalTranslations(CurrentCulture);

        if (localTranslations.Loaded)
        {
            translationUpdater.UpdateTranslations(localTranslations.Localization, localTranslations.Source);
        }

        logger.LogTrace("Loaded local translations from source: {TranslationSource}", localTranslations.Source);

        if (localTranslations.Source is TranslationSource.Internal or TranslationSource.ColdCache)
        {
            logger.LogInformation("External translations can be updated, checking for newer copy...");

            // Check cache and go get up to date translations
            Task.Run(() => CheckForTranslationUpdates(CurrentCulture))
                .Forget();
        }

        return Task.CompletedTask;
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

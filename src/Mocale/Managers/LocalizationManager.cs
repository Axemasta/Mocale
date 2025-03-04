using System.Globalization;
using Ardalis.GuardClauses;
namespace Mocale.Managers;

internal class LocalizationManager : ILocalizationManager
{
    private readonly ICurrentCultureManager currentCultureManager;
    private readonly ILogger logger;
    private readonly IMocaleConfiguration mocaleConfiguration;
    private readonly ITranslationResolver translationResolver;
    private readonly IInternalTranslatorManager translatorManager;

    public CultureInfo CurrentCulture { get; private set; }

    public LocalizationManager(
        ICurrentCultureManager currentCultureManager,
        IConfigurationManager<IMocaleConfiguration> configurationManager,
        ILogger<LocalizationManager> logger,
        ITranslationResolver translationResolver,
        IInternalTranslatorManager translatorManager)
    {
        this.currentCultureManager = Guard.Against.Null(currentCultureManager, nameof(currentCultureManager));
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.translationResolver = Guard.Against.Null(translationResolver, nameof(translationResolver));
        this.translatorManager = Guard.Against.Null(translatorManager, nameof(this.translatorManager));

        configurationManager = Guard.Against.Null(configurationManager, nameof(configurationManager));
        this.mocaleConfiguration = configurationManager.Configuration;

        CurrentCulture = currentCultureManager.GetActiveCulture();
    }

    public async Task<bool> SetCultureAsync(CultureInfo culture)
    {
        try
        {
            bool loaded;

            if (mocaleConfiguration.UseExternalProvider)
            {
                loaded = await TryLoadInternalAndExternalTranslations(culture);
            }
            else
            {
                loaded = TryLoadInternalTranslations(culture);
            }

            if (!loaded)
            {
                logger.LogWarning("Unable to load culture {CultureName}, no localizations found", culture.Name);
                return false;
            }

            translatorManager.RaisePropertyChanged();

            CurrentCulture = culture;

            UpdateThreadCulture(culture);

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

        translatorManager.UpdateTranslations(localTranslations.Localization, localTranslations.Source);

        logger.LogTrace("Loaded local translations from source: {TranslationSource}", localTranslations.Source);

        if (localTranslations.Source is TranslationSource.Internal or TranslationSource.ColdCache && mocaleConfiguration.UseExternalProvider)
        {
            logger.LogInformation("External translations can be updated, checking for newer copy...");

            // Check cache and go get up to date translations
            Task.Run(() => CheckForTranslationUpdates(CurrentCulture))
                .Forget();
        }

        UpdateThreadCulture(CurrentCulture);

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

        translatorManager.UpdateTranslations(external.Localization, TranslationSource.External);
    }

    private bool TryLoadInternalTranslations(CultureInfo culture)
    {
        var localTranslations = translationResolver.LoadLocalTranslations(culture);

        if (localTranslations.Loaded)
        {
            translatorManager.UpdateTranslations(localTranslations.Localization, localTranslations.Source, false);
            return true;
        }
        else
        {
            logger.LogInformation("No internal translations found for culture: {CultureName}, consider adding them as a backup", culture.Name);
            return false;
        }
    }

    private async Task<bool> TryLoadInternalAndExternalTranslations(CultureInfo culture)
    {
        var result = await translationResolver.LoadTranslations(culture);

        if (result.Loaded)
        {
            translatorManager.UpdateTranslations(result.Localization, result.Source, false);
        }
        else
        {
            logger.LogWarning("Unable to load culture {CultureName} from external provider", culture.Name);
        }

        var localTranslations = translationResolver.LoadLocalTranslations(culture);

        if (localTranslations.Loaded)
        {
            translatorManager.UpdateTranslations(localTranslations.Localization, localTranslations.Source, false);
        }
        else
        {
            logger.LogInformation("No internal translations found for culture: {CultureName}, consider adding them as a backup", culture.Name);
        }

        return result.Loaded || localTranslations.Loaded;
    }

    private static void UpdateThreadCulture(CultureInfo cultureInfo)
    {
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}

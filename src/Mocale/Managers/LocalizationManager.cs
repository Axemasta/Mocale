using System.ComponentModel;
using System.Globalization;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
namespace Mocale.Managers;

public class LocalizationManager : ILocalizationManager, INotifyPropertyChanged
{
    private readonly IMocaleConfiguration mocaleConfiguration;
    private readonly ILogger logger;
    private readonly IPreferences preferences;

    public event PropertyChangedEventHandler? PropertyChanged;

    public CultureInfo CurrentCulture { get; private set; }

    private Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();

    private readonly IInternalLocalizationProvider localizationProvider;

    public LocalizationManager(
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        IInternalLocalizationProvider localizationProvider,
        ILogger<LocalizationManager> logger,
        IPreferences preferences)
    {
        mocaleConfigurationManager = Guard.Against.Null(mocaleConfigurationManager, nameof(mocaleConfigurationManager));

        this.mocaleConfiguration = mocaleConfigurationManager.GetConfiguration();
        this.localizationProvider = Guard.Against.Null(localizationProvider, nameof(localizationProvider));
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.preferences = Guard.Against.Null(preferences, nameof(preferences));

        CurrentCulture = GetActiveCulture();
    }

    public object this[string resourceKey]
    {
        get
        {
            if (!Localizations.ContainsKey(resourceKey))
            {
                logger.LogWarning("Resource key not found '{ResourceKey}'", resourceKey);

                if (!mocaleConfiguration.ShowMissingKeys)
                {
                    return string.Empty;
                }

                return mocaleConfiguration.NotFoundSymbol + resourceKey + mocaleConfiguration.NotFoundSymbol;
            }

            return Localizations[resourceKey];
        }
    }


    public async Task<bool> SetCultureAsync(CultureInfo culture)
    {
        try
        {
            var values = localizationProvider.GetValuesForCulture(culture);

            if (values is null || !values.Any())
            {
                logger.LogWarning("Unable to load culture {CultureName}, no localizations found", culture.Name);
                return false;
            }

            Localizations = values;

            CurrentCulture = culture;

            SetActiveCulture(culture);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

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
        Localizations = localizationProvider.GetValuesForCulture(CurrentCulture);

        // Check cache and go get up to date translations

        Task.Run(() => CheckForTranslationUpdates(CurrentCulture))
            .Forget();

        return Task.CompletedTask;
    }

    private async Task CheckForTranslationUpdates(CultureInfo cultureInfo)
    {
        await Task.Delay(5000);

        Localizations.Add("LoadedTranslation", "THIS CAME LATER!");

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        // This will become available 5 seconds after the app loads
    }
}

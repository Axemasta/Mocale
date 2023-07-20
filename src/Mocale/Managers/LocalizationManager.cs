using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Logging;
namespace Mocale.Managers;

public class LocalizationManager : ILocalizationManager, INotifyPropertyChanged
{
    private readonly IMocaleConfiguration mocaleConfiguration;
    private readonly ILogger logger;

    public event PropertyChangedEventHandler PropertyChanged;

    public CultureInfo CurrentCulture { get; set; }

    private Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();

    private readonly IInternalLocalizationProvider localizationProvider;

    public LocalizationManager(
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        IInternalLocalizationProvider localizationProvider,
        ILogger<LocalizationManager> logger)
    {
        mocaleConfiguration = mocaleConfigurationManager.GetConfiguration();
        this.localizationProvider = localizationProvider;
        this.logger = logger;

        CurrentCulture = mocaleConfiguration.DefaultCulture;
    }

    public object this[string resourceKey]
    {
        get
        {
            if (!Localizations.ContainsKey(resourceKey))
            {
                logger.LogWarning("Resource key not found '{0}'", resourceKey);

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
                logger.LogWarning("Unable to load culture {0}, no localizations found", culture.Name);
                return false;
            }

            Localizations = values;

            CurrentCulture = culture;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

            logger.LogDebug("Updated localization culture to {0}", culture.Name);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred loading culture: {0}", culture.Name);

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

    private async Task InitializeInternal()
    {
        // TODO: Lookup selected culture
        var activeCulture = CurrentCulture;

        Localizations = localizationProvider.GetValuesForCulture(activeCulture);

        // Check cache and go get up to date translations

        Task.Run(() => CheckForTranslationUpdates(activeCulture))
            .Forget();
    }

    private async Task CheckForTranslationUpdates(CultureInfo cultureInfo)
    {
        await Task.Delay(5000);

        Localizations.Add("LoadedTranslation", "THIS CAME LATER!");

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        // This will become available 5 seconds after the app loads
    }
}

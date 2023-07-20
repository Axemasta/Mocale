using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;

namespace Mocale.Managers;

public class LocalizationManager : ILocalizationManager, INotifyPropertyChanged
{
    private readonly IMocaleConfiguration mocaleConfiguration;
    private readonly ILogger logger;

    public CultureInfo CurrentCulture { get; set; }

    private Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();

    private readonly IInternalLocalizationProvider localizationProvider;

    public LocalizationManager(
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        IInternalLocalizationProvider localizationProvider,
        ILogger<LocalizationManager> logger)
    {
        this.mocaleConfiguration = mocaleConfigurationManager.GetConfiguration();
        this.localizationProvider = localizationProvider;
        this.logger = logger;

        CurrentCulture = mocaleConfiguration.DefaultCulture;

        Localizations = localizationProvider.GetValuesForCulture(CurrentCulture);
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

    public event PropertyChangedEventHandler PropertyChanged;

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
}

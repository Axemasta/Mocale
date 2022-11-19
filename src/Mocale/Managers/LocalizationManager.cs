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

    private readonly ILocalizationProvider localizationProvider;

    internal static ILocalizationManager Instance { get; private set; }

    public LocalizationManager(
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        ILocalizationProvider localizationProvider,
        ILogger<LocalizationManager> logger)
    {
        this.mocaleConfiguration = mocaleConfigurationManager.GetConfiguration();
        this.localizationProvider = localizationProvider;
        this.logger = logger;

        CurrentCulture = mocaleConfiguration.DefaultCulture;

        Localizations = localizationProvider.GetValuesForCulture(CurrentCulture);

        //
        Instance = this;
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

    public void SetCulture(CultureInfo culture)
    {
        //AppResources.Culture = culture;
        CurrentCulture = culture;
        Localizations = localizationProvider.GetValuesForCulture(CurrentCulture);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        logger.LogDebug("Updated localization culture to {0}", culture.Name);
    }
}

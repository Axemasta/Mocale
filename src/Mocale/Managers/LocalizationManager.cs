using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Services;

namespace Mocale.Managers
{
    public class LocalizationManager : ILocalizationManager, INotifyPropertyChanged
    {
        private readonly IMocaleConfiguration configuration;
        private readonly ILogger logger;

        public CultureInfo CurrentCulture { get; set; }

        private Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();

        private readonly ILocalizationProvider localizationProvider;

        public LocalizationManager(
            IConfigurationManager configurationManager,
            ILocalizationProvider localizationProvider,
            ILogger<LocalizationManager> logger)
        {
            this.configuration = configurationManager.GetConfiguration();
            this.localizationProvider = localizationProvider;
            this.logger = logger;

            CurrentCulture = configuration.DefaultCulture;

            Localizations = localizationProvider.GetValuesForCulture(CurrentCulture);
        }

        // Using a service locator here is just criminal, need to init when the library is all registered...
        // Maybe a .Build() method on the apphostbuilder to commit everything once config has occurred?
        public static ILocalizationManager Instance
        {
            get
            {
                return AppBuilderExtensions.ServiceProvider.GetRequiredService<ILocalizationManager>();
            }
        }

        public object this[string resourceKey]
        {
            get
            {
                if (!Localizations.ContainsKey(resourceKey))
                {
                    logger.LogWarning("Resource key not found '{0}'", resourceKey);

                    if (!configuration.ShowMissingKeys)
                    {
                        return string.Empty;
                    }

                    return configuration.NotFoundSymbol + resourceKey + configuration.NotFoundSymbol;
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
}

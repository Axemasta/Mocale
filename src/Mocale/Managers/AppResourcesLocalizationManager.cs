using System.ComponentModel;
using System.Globalization;
using Mocale.Abstractions;
using Mocale.Services;

namespace Mocale.Managers
{
    public class LocalizationManager : ILocalizationManager, INotifyPropertyChanged
    {
        private readonly IMocaleConfiguration configuration;

        public CultureInfo CurrentCulture { get; set; }

        private Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();

        private readonly ILocalizationProvider localizationProvider;

        private LocalizationManager(IConfigurationManager configurationManager)
        {
            this.configuration = configurationManager.GetConfiguration();

            CurrentCulture = configuration.DefaultCulture;

            localizationProvider = MocaleBuilder.Instance.LocalizationProvider;

            Localizations = localizationProvider.GetValuesForCulture(CurrentCulture);
        }

        public static LocalizationManager Instance { get; } = new(ConfigurationManager.Instance);

        public object this[string resourceKey] => (object)Localizations[resourceKey] ?? Array.Empty<byte>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetCulture(CultureInfo culture)
        {
            //AppResources.Culture = culture;
            CurrentCulture = culture;
            Localizations = localizationProvider.GetValuesForCulture(CurrentCulture);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}

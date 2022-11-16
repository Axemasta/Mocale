using System.ComponentModel;
using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Managers
{
    public class LocalizationManager : ILocalizationManager, INotifyPropertyChanged
    {
        private CultureInfo CurrentCulture { get; set; }

        private Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();

        private readonly ILocalizationProvider localizationProvider;

        private LocalizationManager()
        {
            localizationProvider = MocaleBuilder.Instance.LocalizationProvider;

            // TODO: Default to thread? Get from config?
            var defaultCulture = new CultureInfo("en-GB");

            Localizations = localizationProvider.GetValuesForCulture(defaultCulture);
        }

        public static LocalizationManager Instance { get; } = new();

        public object this[string resourceKey] => (object)Localizations[resourceKey] ?? Array.Empty<byte>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetCulture(CultureInfo culture)
        {
            //AppResources.Culture = culture;
            CurrentCulture = culture;
            Localizations = localizationProvider.GetValuesForCulture(culture);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}


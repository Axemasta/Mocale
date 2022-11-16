using System.Globalization;
using Mocale.Managers;
using MvvmHelpers;

namespace Mocale.Samples.ViewModels
{
    internal class IntroductionPageViewModel : BaseViewModel
    {
        public ObservableRangeCollection<string> Locales { get; }

        private string selectedLocale;

        public string SelectedLocale
        {
            get => selectedLocale;
            set
            {
                var oldValue = selectedLocale;

                if (SetProperty(ref selectedLocale, value))
                {
                    RaiseLocaleSelected(oldValue, value);
                }
            }
        }

        public IntroductionPageViewModel()
        {
            Locales = new ObservableRangeCollection<string>(new string[]
            {
                "en-GB",
                "fr-FR",
            });

            SelectedLocale = Locales[0];
        }

        private void RaiseLocaleSelected(string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(oldValue))
            {
                return;
            }

            if (oldValue.Equals(newValue))
                return;

            var culture = new CultureInfo(newValue);

            LocalizationManager.Instance.SetCulture(culture);
        }
    }
}

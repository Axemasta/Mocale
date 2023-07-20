using System.Globalization;
using Mocale.Abstractions;
using MvvmHelpers;

namespace Mocale.Samples.ViewModels;

public class IntroductionPageViewModel : BaseViewModel
{
    private readonly ILocalizationManager localizationManager;

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

    public IntroductionPageViewModel(ILocalizationManager localizationManager)
    {
        this.localizationManager = localizationManager;

        Locales = new ObservableRangeCollection<string>(new string[]
        {
            "en-GB",
            "fr-FR",
            "it-IT",
        });

        var selectedLocale = Locales.FirstOrDefault(localizationManager.CurrentCulture.Name.Equals);

        SelectedLocale = selectedLocale ?? Locales[0];
    }

    private async void RaiseLocaleSelected(string oldValue, string newValue)
    {
        if (string.IsNullOrEmpty(oldValue))
        {
            return;
        }

        if (oldValue.Equals(newValue, StringComparison.Ordinal))
        {
            return;
        }

        var culture = new CultureInfo(newValue);

        if (culture.Equals(localizationManager.CurrentCulture))
        {
            return;
        }

        var loaded = await localizationManager.SetCultureAsync(culture);

        if (!loaded)
        {
            SelectedLocale = oldValue;
        }
    }
}

using System.Globalization;

namespace Mocale.Samples.ViewModels;

public partial class IntroductionPageViewModel : BaseViewModel
{
    private readonly ILocalizationManager localizationManager;

    public ObservableRangeCollection<string> Locales { get; }

    [ObservableProperty]
    public partial string SelectedLocale { get; set; }

    public IntroductionPageViewModel(ILocalizationManager localizationManager)
    {
        this.localizationManager = localizationManager;

        Locales = new ObservableRangeCollection<string>(
        [
            "en-GB",
            "fr-FR",
            "it-IT",
        ]);

        var selectedLocale = Locales.FirstOrDefault(localizationManager.CurrentCulture.Name.Equals);

        SelectedLocale = selectedLocale ?? Locales[0];
    }

    partial void OnSelectedLocaleChanged(string oldValue, string newValue)
    {
        RaiseLocaleSelected(oldValue, newValue);
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

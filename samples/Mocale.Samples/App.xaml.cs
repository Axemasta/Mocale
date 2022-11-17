using Mocale.Managers;
using Mocale.Samples.ViewModels;
using Mocale.Samples.Views;

namespace Mocale.Samples;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var introPage = new IntroductionPage()
        {
            // TODO: Register the bits consuming apps might want to access in the service collection?
            BindingContext = new IntroductionPageViewModel(LocalizationManager.Instance),
        };

        MainPage = new NavigationPage(introPage);
    }
}

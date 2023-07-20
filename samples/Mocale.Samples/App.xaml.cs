using Mocale.Abstractions;
using Mocale.Samples.ViewModels;
using Mocale.Samples.Views;

namespace Mocale.Samples;

public partial class App : Application, IAppServiceProvider
{
    public IServiceProvider ServiceProvider { get; }

    public App(
        IServiceProvider serviceProvider,
        ILocalizationManager localizationManager)
    {
        this.ServiceProvider = serviceProvider;

        InitializeComponent();

        var introPage = new IntroductionPage()
        {
            // TODO: Register the bits consuming apps might want to access in the service collection?
            BindingContext = new IntroductionPageViewModel(localizationManager),
        };

        MainPage = new NavigationPage(introPage);
    }
}

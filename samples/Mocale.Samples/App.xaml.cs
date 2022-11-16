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
            BindingContext = new IntroductionPageViewModel(),
        };

        MainPage = new NavigationPage(introPage);
    }
}

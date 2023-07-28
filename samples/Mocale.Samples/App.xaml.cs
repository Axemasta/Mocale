using Mocale.Samples.Views;
namespace Mocale.Samples;

public partial class App : Application
{
    public App(IntroductionPage introductionPage)
    {
        InitializeComponent();

        MainPage = new NavigationPage(introductionPage);
    }
}

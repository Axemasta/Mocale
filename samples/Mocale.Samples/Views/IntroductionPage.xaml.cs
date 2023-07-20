using Mocale.Samples.ViewModels;
namespace Mocale.Samples.Views;

public partial class IntroductionPage : ContentPage
{
    public IntroductionPage(IntroductionPageViewModel introductionPageViewModel)
    {
        BindingContext = introductionPageViewModel;

        InitializeComponent();
    }
}

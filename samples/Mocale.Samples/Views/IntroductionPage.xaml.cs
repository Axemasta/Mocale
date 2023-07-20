using Mocale.Samples.ViewModels;
namespace Mocale.Samples.Views;

public partial class IntroductionPage : ContentPage
{
    public IntroductionPage(IntroductionPageViewModel introductionPageViewModel)
    {
        this.BindingContext = introductionPageViewModel;

        InitializeComponent();
    }
}

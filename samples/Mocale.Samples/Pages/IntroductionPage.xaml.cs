using Mocale.Samples.ViewModels;
namespace Mocale.Samples.Pages;

public partial class IntroductionPage : BasePage<IntroductionPageViewModel>
{
    public IntroductionPage(IntroductionPageViewModel viewModel)
        : base(viewModel)
    {

        InitializeComponent();
    }
}

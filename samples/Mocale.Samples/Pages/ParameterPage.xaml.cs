using Mocale.Samples.ViewModels;

namespace Mocale.Samples.Pages;

public partial class ParameterPage : BasePage<ParameterViewModel>
{
    public ParameterPage(ParameterViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}

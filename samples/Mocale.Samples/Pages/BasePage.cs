using CommunityToolkit.Mvvm.ComponentModel;

namespace Mocale.Samples.Pages;

public abstract class BasePage<TViewModel> : ContentPage where TViewModel : ObservableObject
{
    protected BasePage(TViewModel viewModel)
    {
        base.BindingContext = viewModel;
    }

    public new TViewModel BindingContext => (TViewModel)base.BindingContext;
}

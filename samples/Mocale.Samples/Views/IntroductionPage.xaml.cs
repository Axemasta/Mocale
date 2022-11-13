using Mocale.Samples.MoveToLib;

namespace Mocale.Samples.Views;

public partial class IntroductionPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager { get; }

    public IntroductionPage()
	{
		InitializeComponent();

        LocalizationResourceManager = LocalizationResourceManager.Instance;
    }
}
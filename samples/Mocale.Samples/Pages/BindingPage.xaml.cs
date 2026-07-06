using Mocale.Extensions;

namespace Mocale.Samples.Pages;

public partial class BindingPage : ContentPage
{
	public BindingPage()
	{
		InitializeComponent();

		Label.SetTranslation(Label.TextProperty, TranslationKeys.BindingPageCodeBehindLabelKey);
	}
}

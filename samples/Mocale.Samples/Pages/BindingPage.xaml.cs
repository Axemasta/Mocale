using Mocale.Extensions;
using Mocale.Translations;

namespace Mocale.Samples.Pages;

public partial class BindingPage : ContentPage
{
    public BindingPage()
    {
        InitializeComponent();

        Label.SetTranslation(Label.TextProperty, TranslationKeys.BindingPageCodeBehindLabelKey);
    }
}

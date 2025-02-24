using CommunityToolkit.Maui.Markup;
using Mocale.Extensions;
using Mocale.Samples.Enums;
using Mocale.Translations;

namespace Mocale.Samples.Pages;

internal sealed partial class CodePage : ContentPage
{
    public CodePage()
    {
        this.SetTranslation(TitleProperty, TranslationKeys.CodePageTitle);


        var cityPicker = new Picker
        {
            ItemsSource = new List<Cities>
            {
                Cities.London,
                Cities.Manchester,
                Cities.Nottingham,
                Cities.Newcastle
            }
        }.SetTranslation(Picker.TitleProperty, TranslationKeys.BindingPageCityDemoPickerPlaceholder);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                Children =
                {
                    new Label().SetTranslation(Label.TextProperty, TranslationKeys.BindingPageHeading),

                    new Border
                    {
                        Content = new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label().SetTranslation(Label.TextProperty, TranslationKeys.BindingPageCityDemoTitle),
                                new Label().SetTranslation(Label.TextProperty, TranslationKeys.BindingPageCityDemoHeading),
                                cityPicker,
                                new Label().SetEnumTranslation(Label.TextProperty, new Binding(nameof(Picker.SelectedItem), source: cityPicker)),
                            }
                        }
                    },

                    new Border
                    {
                        Content = new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label().SetTranslation(Label.TextProperty, TranslationKeys.BindingPageCodeBehindTitle),
                                new Label().SetTranslation(Label.TextProperty, TranslationKeys.BindingPageCodeBehindHeading),
                            }
                        }
                    }
                }
            }
        };
    }
}

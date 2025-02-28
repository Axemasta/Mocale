using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Managers;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Collections;

namespace Mocale.UnitTests.Extensions;

[Collection(CollectionNames.MocaleLocatorTests)]
public partial class BindableObjectExtensionTests
{
    #region Setup

    private readonly TranslatorManagerProxy translatorManager = new();

    public BindableObjectExtensionTests()
    {
        MocaleLocator.SetInstance(translatorManager);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void SetTranslationVoid_WhenBindableObjectIsNull_ShouldThrow()
    {
        // Arrange
        BindableObject bindableObject = null!;

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => bindableObject.SetTranslation(Label.TextProperty, "ApplicationTitle"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetTranslationVoid_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        ((BindableObject)label).SetTranslation(Label.TextProperty, "ApplicationTitle");

        // Assert
        Assert.Equal("$ApplicationTitle$", label.Text);
    }

    [Fact]
    public void SetTranslationVoid_WhenTranslationKeyExists_ShouldSetTranslation()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "ApplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        ((BindableObject)label).SetTranslation(Label.TextProperty, "ApplicationTitle", null);

        // Assert
        Assert.Equal("Mocale!", label.Text);
    }

    [Fact]
    public void SetTranslationVoid_WhenLocaleChanges_ShouldUpdateTranslation()
    {
        // Arrange
        var enGbLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World!"}
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();
        ((BindableObject)label).SetTranslation(Label.TextProperty, "HelloWorld", null);
        Assert.Equal("Hello World!", label.Text);

        // Act
        var frFrLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("fr-FR"),
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Bonjour le monde!!"}
            }
        };

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("Bonjour le monde!!", label.Text);
    }

    [Fact]
    public void SetTranslationVoid_WhenUsingConverter_ShouldTranslateAndUseConverter()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "ApplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        ((BindableObject)label).SetTranslation(Label.TextProperty, "ApplicationTitle", new UpperCaseConverter());

        // Assert
        Assert.Equal("MOCALE!", label.Text);
    }

    [Fact]
    public void SetTranslationView_WhenViewIsNull_ShouldThrow()
    {
        // Arrange
        Label? label = null;

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => BindableObjectExtension.SetTranslation(label, Label.TextProperty, "ApplicationTitle"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'view')", ex.Message);
    }

    [Fact]
    public void SetTranslationView_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        var view = label.SetTranslation(Label.TextProperty, "ApplicationTitle");

        // Assert
        Assert.Equal("$ApplicationTitle$", label.Text);
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
    }

    [Fact]
    public void SetTranslationView_WhenTranslationKeyExists_ShouldSetTranslation()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "ApplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        var view = label.SetTranslation(Label.TextProperty, "ApplicationTitle");

        // Assert
        Assert.Equal("Mocale!", label.Text);
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
    }

    [Fact]
    public void SetTranslationView_WhenUsingConverter_ShouldTranslateAndUseConverter()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "ApplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        var view = label.SetTranslation(Label.TextProperty, "ApplicationTitle", new UpperCaseConverter());

        // Assert
        Assert.Equal("MOCALE!", label.Text);
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenBindableObjectIsNull_ShouldThrow()
    {
        // Arrange
        var viewModel = new SomeViewModel();

        BindableObject bindableObject = null!;
        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => bindableObject.SetTranslationBinding(Label.TextProperty, binding, "ApplicationTitle"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenBindingIsNull_ShouldThrow()
    {
        // Arrange
        BindableObject bindableObject = new Label();

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => bindableObject.SetTranslationBinding(Label.TextProperty, null!, "ApplicationTitle"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'source')", ex.Message);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!"},
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var viewModel = new SomeViewModel();
        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        BindableObject bindableObject = new Label();

        // Act
        bindableObject.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("$CurrentTemperatureLabel$", label.Text);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenTranslationKeyExistsForOneCulture_ShouldFormatTranslation()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "The temperature is {0}\u00b0C" }
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var viewModel = new SomeViewModel
        {
            Temperature = 21.3
        };

        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        BindableObject bindableObject = new Label();

        // Act
        bindableObject.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        var enGbLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "The temperature is {0}\u00b0C" }
            }
        };

        var frFrLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("fr-FR"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel
        {
            Temperature = 21.3
        };

        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        BindableObject bindableObject = new Label();

        // Act & Assert
        bindableObject.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        Assert.Equal("$CurrentTemperatureLabel$", label.Text);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenTranslationKeyExistsForAllCultures_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        var enGbLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "The temperature is {0}\u00b0C" }
            }
        };

        var frFrLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("fr-FR"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "La température est {0}\u00b0C" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel
        {
            Temperature = 21.3
        };

        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        BindableObject bindableObject = new Label();

        // Act & Assert
        bindableObject.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        Assert.Equal("La température est 21.3\u00b0C", label.Text);
    }

    #endregion Tests

    #region Test Data

    private class UpperCaseConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string str
                ? str.ToUpper()
                : null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    private partial class SomeViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial double Temperature { get; set; }
    }

    #endregion Test Data
}

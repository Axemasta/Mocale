using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Collections;
using Mocale.UnitTests.Fixtures;

namespace Mocale.UnitTests.Extensions;

[Collection(CollectionNames.MocaleLocatorTests)]
public partial class BindableObjectExtensionTests : ControlsFixtureBase
{
    #region Setup

    private readonly TranslatorManagerProxy translatorManager = new();

    public BindableObjectExtensionTests()
    {
        MocaleLocator.SetInstance(translatorManager);
    }

    #endregion Setup

    #region Tests

    #region - SetTranslation

    #region -- Void

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
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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
        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "ApplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        ((BindableObject)label).SetTranslation(Label.TextProperty, "ApplicationTitle");

        // Assert
        Assert.Equal("Mocale!", label.Text);
    }

    [Fact]
    public void SetTranslationVoid_WhenLocaleChanges_ShouldUpdateTranslation()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World!"}
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();
        ((BindableObject)label).SetTranslation(Label.TextProperty, "HelloWorld");
        Assert.Equal("Hello World!", label.Text);

        // Act
        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
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
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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

    #endregion -- Void

    #region -- View

    [Fact]
    public void SetTranslationView_WhenViewIsNull_ShouldThrow()
    {
        // Arrange
        Label label = null!;

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => label.SetTranslation(Label.TextProperty, "ApplicationTitle"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetTranslationView_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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

    #endregion -- View

    #endregion - SetTranslation

    #region - SetTranslationBinding

    #region -- Void

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
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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
    public void SetTranslationBindingVoid_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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
    public void SetTranslationBindingVoid_WhenBindedValueChanges_ShouldUpdatedTranslation()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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

        bindableObject.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);

        // Act
        viewModel.Temperature = 37.8;

        // Assert
        Assert.Equal("The temperature is 37.8\u00b0C", label.Text);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenBindedValueString_ShouldUpdatedTranslation()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "WelcomeMessage", "Hello {0}, Welcome Back!" }
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var viewModel = new SomeViewModel
        {
            Temperature = 21.3,
            Name = "Morty"
        };

        var binding = new Binding(nameof(SomeViewModel.Name), source: viewModel);

        BindableObject bindableObject = new Entry();

        // Act
        bindableObject.SetTranslationBinding(Entry.TextProperty, binding, "WelcomeMessage");

        // Assert
        var entry = Assert.IsType<Entry>(bindableObject);
        Assert.Equal("Hello Morty, Welcome Back!", entry.Text);
    }

    [Fact]
    public void SetTranslationBindingVoid_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "The temperature is {0}\u00b0C" }
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
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
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "The temperature is {0}\u00b0C" }
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "La température est {0} \u00b0C" }
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

        Assert.Equal("La température est 21.3 \u00b0C", label.Text);
    }

    #endregion -- Void

    #region -- View

    [Fact]
    public void SetTranslationBindingView_WhenBindableObjectIsNull_ShouldThrow()
    {
        // Arrange
        var viewModel = new SomeViewModel();

        View view = null!;
        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => view.SetTranslationBinding(Label.TextProperty, binding, "ApplicationTitle"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetTranslationBindingView_WhenBindingIsNull_ShouldThrow()
    {
        // Arrange
        var view = new Label();

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => view.SetTranslationBinding(Label.TextProperty, null!, "ApplicationTitle"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'source')", ex.Message);
    }

    [Fact]
    public void SetTranslationBindingView_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!"},
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var viewModel = new SomeViewModel();
        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        var label = new Label();

        // Act
        var view = label.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        // Assert
        Assert.Equal("$CurrentTemperatureLabel$", label.Text);
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
    }

    [Fact]
    public void SetTranslationBindingView_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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

        var label = new Label();

        // Act
        var view = label.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        // Assert
        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
    }

    [Fact]
    public void SetTranslationBindingView_WhenBindedValueChanges_ShouldUpdatedTranslation()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
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

        var label = new Label();

        var view = label.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);

        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);

        // Act
        viewModel.Temperature = 37.8;

        // Assert
        Assert.Equal("The temperature is 37.8\u00b0C", label.Text);
    }

    [Fact]
    public void SetTranslationBindingView_WhenBindedValueString_ShouldUpdatedTranslation()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "WelcomeMessage", "Hello {0}, Welcome Back!" }
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var viewModel = new SomeViewModel
        {
            Temperature = 21.3,
            Name = "Morty"
        };

        var binding = new Binding(nameof(SomeViewModel.Name), source: viewModel);

        var entry = new Entry();

        // Act
        var view = entry.SetTranslationBinding(Entry.TextProperty, binding, "WelcomeMessage");

        // Assert
        Assert.Equal("Hello Morty, Welcome Back!", entry.Text);
        Assert.IsType<Entry>(view);
        Assert.Equal(entry, view);
    }

    [Fact]
    public void SetTranslationBindingView_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "The temperature is {0}\u00b0C" }
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
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

        var label = new Label();

        // Act & Assert
        var view = label.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        Assert.Equal("$CurrentTemperatureLabel$", label.Text);
    }

    [Fact]
    public void SetTranslationBindingView_WhenTranslationKeyExistsForAllCultures_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "The temperature is {0}\u00b0C" }
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
                { "CurrentTemperatureLabel", "La température est {0} \u00b0C" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel
        {
            Temperature = 21.3
        };

        var binding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);

        var label = new Label();

        // Act & Assert
        var view = label.SetTranslationBinding(Label.TextProperty, binding, "CurrentTemperatureLabel");

        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("The temperature is 21.3\u00b0C", label.Text);

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        Assert.Equal("La température est 21.3 \u00b0C", label.Text);
    }

    #endregion -- View

    #endregion - SetTranslationBinding

    #region - SetEnumTranslation

    #region -- Void

    [Fact]
    public void SetEnumTranslationVoid_WhenBindableObjectIsNull_ShouldThrow()
    {
        // Arrange
        var viewModel = new SomeViewModel();

        BindableObject bindableObject = null!;
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => bindableObject.SetEnumTranslation(Label.TextProperty, binding));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenBindingIsNull_ShouldThrow()
    {
        // Arrange
        BindableObject bindableObject = new Label();
        Binding binding = null!;

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => bindableObject.SetEnumTranslation(Label.TextProperty, binding));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'source')", ex.Message);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenConfiguredNotToUseAttribute_ShouldEnumString()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = false,
            }
        };

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        BindableObject bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Apple", label.Text);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        BindableObject bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("$Fruit_Apple$", label.Text);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        BindableObject bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Banana", label.Text);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenConverterIsSet_ShouldFormatTranslationUsingConverter()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        BindableObject bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), BindingMode.Default, new UpperCaseWithNumberConverter(), 1337, null, viewModel);

        // Act
        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("APPLE_1337", label.Text);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenBindedValueChanges_ShouldUpdateTranslation()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        BindableObject bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act & Assert
        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Banana", label.Text);

        viewModel.SelectedFruit = Fruit.Cherry;

        Assert.Equal("Cherry", label.Text);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NotFruit", "Ce n'est pas un fruit" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        BindableObject bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("$Fruit_Banana$", label.Text);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenTranslationKeyExistsForAllCultures_ShouldUpdateTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Pomme" },
                { "Fruit_Banana", "Banane" },
                { "Fruit_Cherry", "Cerise" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        BindableObject bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("Banane", label.Text);
    }

    [Fact]
    public void SetEnumTranslationVoid_WhenConverterIsSetAndCultureChanges_ShouldUpdateCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        var bindableObject = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), BindingMode.Default, new UpperCaseWithNumberConverter(), 73, null, viewModel);

        bindableObject.SetEnumTranslation(Label.TextProperty, binding);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("APPLE_73", label.Text);

        // Act
        var itITLocalization = new Localization(new CultureInfo("it-IT"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Mela" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Ciliegia" },
            }
        };

        translatorManager.UpdateTranslations(itITLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("MELA_73", label.Text);
    }

    #endregion -- Void

    #region -- View

    [Fact]
    public void SetEnumTranslationView_WhenBindableObjectIsNull_ShouldThrow()
    {
        // Arrange
        var viewModel = new SomeViewModel();

        Label label = null!;
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => label.SetEnumTranslation(Label.TextProperty, binding));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetEnumTranslationView_WhenBindingIsNull_ShouldThrow()
    {
        // Arrange
        var label = new Label();

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => label.SetEnumTranslation(Label.TextProperty, null!));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'source')", ex.Message);
    }

    [Fact]
    public void SetEnumTranslationView_WhenConfiguredNotToUseAttribute_ShouldEnumString()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = false,
            }
        };

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Apple", label.Text);
    }

    [Fact]
    public void SetEnumTranslationView_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("$Fruit_Apple$", label.Text);
    }

    [Fact]
    public void SetEnumTranslationView_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Banana", label.Text);
    }

    [Fact]
    public void SetEnumTranslationView_WhenConverterIsSet_ShouldFormatTranslationUsingConverter()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), BindingMode.Default, new UpperCaseWithNumberConverter(), 182, null, viewModel);

        // Act
        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal("APPLE_182", label.Text);
    }

    [Fact]
    public void SetEnumTranslationView_WhenBindedValueChanges_ShouldUpdateTranslation()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act & Assert
        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Banana", label.Text);

        viewModel.SelectedFruit = Fruit.Cherry;

        Assert.Equal("Cherry", label.Text);
    }

    [Fact]
    public void SetEnumTranslationView_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NotFruit", "Ce n'est pas un fruit" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("$Fruit_Banana$", label.Text);
    }

    [Fact]
    public void SetEnumTranslationView_WhenTranslationKeyExistsForAllCultures_ShouldUpdateTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Pomme" },
                { "Fruit_Banana", "Banane" },
                { "Fruit_Cherry", "Cerise" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Banana,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("Banane", label.Text);
    }

    [Fact]
    public void SetEnumTranslationView_WhenConverterIsSetAndCultureChanges_ShouldUpdateCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var viewModel = new SomeViewModel()
        {
            SelectedFruit = Fruit.Apple,
        };

        var label = new Label();
        var binding = new Binding(nameof(SomeViewModel.SelectedFruit), BindingMode.Default, new UpperCaseWithNumberConverter(), 73, null, viewModel);

        var view = label.SetEnumTranslation(Label.TextProperty, binding);

        Assert.IsType<Label>(view);
        Assert.Equal("APPLE_73", label.Text);

        // Act
        var itITLocalization = new Localization(new CultureInfo("it-IT"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Mela" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Ciliegia" },
            }
        };

        translatorManager.UpdateTranslations(itITLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("MELA_73", label.Text);
    }

    #endregion -- View

    #endregion - SetEnumTranslation

    #region - SetEnumValueTranslation

    #region -- Void

    [Fact]
    public void SetEnumValueTranslationVoid_WhenConfiguredNotToUseAttribute_ShouldEnumString()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = false,
            }
        };

        BindableObject bindableObject = new Label();

        // Act
        bindableObject.SetEnumValueTranslation(Label.TextProperty, Fruit.Apple);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Apple", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationVoid_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        BindableObject bindableObject = new Label();

        // Act
        bindableObject.SetEnumValueTranslation(Label.TextProperty, Fruit.Apple);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("$Fruit_Apple$", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationVoid_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        BindableObject bindableObject = new Label();

        // Act
        bindableObject.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Banana", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationVoid_WhenConverterIsSet_ShouldFormatTranslationUsingConverter()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        BindableObject bindableObject = new Label();

        // Act
        bindableObject.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana, "{0}", new UpperCaseWithNumberConverter(), 124);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("BANANA_124", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationVoid_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NotFruit", "Ce n'est pas un fruit" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        BindableObject bindableObject = new Label();

        bindableObject.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("$Fruit_Banana$", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationVoid_WhenTranslationKeyExistsForAllCultures_ShouldUpdateTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Pomme" },
                { "Fruit_Banana", "Banane" },
                { "Fruit_Cherry", "Cerise" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        BindableObject bindableObject = new Label();

        bindableObject.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("Banane", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationVoid_WhenConverterIsSetAndCultureChanges_ShouldUpdateCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        BindableObject bindableObject = new Label();

        bindableObject.SetEnumValueTranslation(Label.TextProperty, Fruit.Cherry, "{0}", new UpperCaseWithNumberConverter(), 54);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("CHERRY_54", label.Text);

        // Act
        var itITLocalization = new Localization(new CultureInfo("it-IT"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Mela" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Ciliegia" },
            }
        };

        translatorManager.UpdateTranslations(itITLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("CILIEGIA_54", label.Text);
    }

    #endregion -- Void

    #region -- View

    [Fact]
    public void SetEnumValueTranslationView_WhenConfiguredNotToUseAttribute_ShouldEnumString()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = false,
            }
        };

        var label = new Label();

        // Act
        var view = label.SetEnumValueTranslation(Label.TextProperty, Fruit.Apple);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Apple", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationView_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();

        // Act
        var view = label.SetEnumValueTranslation(Label.TextProperty, Fruit.Apple);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("$Fruit_Apple$", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationView_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();

        // Act
        var view = label.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Banana", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationView_WhenConverterIsSet_ShouldFormatTranslationUsingConverter()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();

        // Act
        var view = label.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana, "{0}", new UpperCaseWithNumberConverter(), 124);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal("BANANA_124", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationView_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NotFruit", "Ce n'est pas un fruit" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();

        var view = label.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana);

        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("$Fruit_Banana$", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationView_WhenTranslationKeyExistsForAllCultures_ShouldUpdateTranslationCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Pomme" },
                { "Fruit_Banana", "Banane" },
                { "Fruit_Cherry", "Cerise" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();

        var view = label.SetEnumValueTranslation(Label.TextProperty, Fruit.Banana);

        Assert.IsType<Label>(view);
        Assert.Equal(label, view);
        Assert.Equal("Banana", label.Text);

        // Act
        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("Banane", label.Text);
    }

    [Fact]
    public void SetEnumValueTranslationView_WhenConverterIsSetAndCultureChanges_ShouldUpdateCorrectly()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = new MocaleConfiguration()
        {
            EnumBehavior = new LocalizeEnumBehavior()
            {
                UseAttribute = true,
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                LocalizeAttribute = typeof(DescriptionAttribute),
            }
        };

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Apple" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Cherry" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();

        var view = label.SetEnumValueTranslation(Label.TextProperty, Fruit.Cherry, "{0}", new UpperCaseWithNumberConverter(), 54);

        Assert.IsType<Label>(view);
        Assert.Equal("CHERRY_54", label.Text);

        // Act
        var itITLocalization = new Localization(new CultureInfo("it-IT"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Fruit_Apple", "Mela" },
                { "Fruit_Banana", "Banana" },
                { "Fruit_Cherry", "Ciliegia" },
            }
        };

        translatorManager.UpdateTranslations(itITLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("CILIEGIA_54", label.Text);
    }

    #endregion -- View

    #endregion - SetEnumValueTranslation

    #region - SetTranslationMultiBinding

    #region -- Void

    [Fact]
    public void SetTranslationMultiBindingVoid_WhenBindableObjectIsNull_ShouldThrow()
    {
        // Arrange
        var viewModel = new SomeViewModel();

        BindableObject bindableObject = null!;
        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => bindableObject.SetTranslationMultiBinding(Label.TextProperty, "NameAndTemperatureLabel", [temperatureBinding, nameBinding]));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetTranslationMultiBindingVoid_WhenBindingsAreEmpty_ShouldThrow()
    {
        // Arrange
        BindableObject bindableObject = new Label();

        // Act
        var ex = Assert.Throws<ArgumentException>(() => bindableObject.SetTranslationMultiBinding(Label.TextProperty, "NameAndTemperatureLabel", []));

        // Assert
        Assert.Equal("Required input Bindings was empty. (Parameter 'Bindings')", ex.Message);
    }

    [Fact]
    public void SetTranslationMultiBindingVoid_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        BindableObject bindableObject = new Label();

        var viewModel = new SomeViewModel();
        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        bindableObject.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [temperatureBinding, nameBinding, fruitBinding]);

        // Assert
        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("$NameTemperatureAndFruitLabel$", label.Text);
    }

    [Fact]
    public void SetTranslationMultiBindingVoid_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        BindableObject bindableObject = new Label();

        var viewModel = new SomeViewModel();
        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} likes {1}s when its {2} outside" }
            }
        };

        bindableObject.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [nameBinding, fruitBinding, temperatureBinding]);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("$NameTemperatureAndFruitLabel$", label.Text);

        // Act
        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal(" likes s when its 0 outside", label.Text);

        viewModel.Name = "Billy";

        Assert.Equal("Billy likes s when its 0 outside", label.Text);

        viewModel.SelectedFruit = Fruit.Apple;

        Assert.Equal("Billy likes Apples when its 0 outside", label.Text);

        viewModel.Temperature = 24.5;

        Assert.Equal("Billy likes Apples when its 24.5 outside", label.Text);
    }

    [Fact]
    public void SetTranslationMultiBindingVoid_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        BindableObject bindableObject = new Label();

        var viewModel = new SomeViewModel()
        {
            Name = "Jimmy",
            SelectedFruit = Fruit.Cherry,
            Temperature = 19.2,
        };

        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} likes {1}s when its {2} outside" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        bindableObject.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [nameBinding, fruitBinding, temperatureBinding]);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Jimmy likes Cherrys when its 19.2 outside", label.Text);

        // Act
        var frFrLocalization = new Localization(new CultureInfo("fr-Fr"));

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("$NameTemperatureAndFruitLabel$", label.Text);
    }

    [Fact]
    public void SetTranslationMultiBindingVoid_WhenTranslationKeyExistsForAllCultures_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        BindableObject bindableObject = new Label();

        var viewModel = new SomeViewModel()
        {
            Name = "Jimmy",
            SelectedFruit = Fruit.Cherry,
            Temperature = 19.2,
        };

        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} likes {1}s when its {2} outside" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        bindableObject.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [nameBinding, fruitBinding, temperatureBinding]);

        var label = Assert.IsType<Label>(bindableObject);
        Assert.Equal("Jimmy likes Cherrys when its 19.2 outside", label.Text);

        // Act
        var frFrLocalization = new Localization(new CultureInfo("fr-Fr"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} aime les {1} quand il est {2} dehors" }
            }
        };

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("Jimmy aime les Cherry quand il est 19.2 dehors", label.Text);
    }

    #endregion -- Void

    #region -- View

    [Fact]
    public void SetTranslationMultiBindingView_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        var view = new Label();

        var viewModel = new SomeViewModel();
        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        // Act
        view.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [temperatureBinding, nameBinding, fruitBinding]);

        // Assert
        Assert.IsType<Label>(view);
        Assert.Equal("$NameTemperatureAndFruitLabel$", view.Text);
    }

    [Fact]
    public void SetTranslationMultiBindingView_WhenTranslationKeyExists_ShouldFormatTranslation()
    {
        // Arrange
        var view = new Label();

        var viewModel = new SomeViewModel();
        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} likes {1}s when its {2} outside" }
            }
        };

        view.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [nameBinding, fruitBinding, temperatureBinding]);

        Assert.IsType<Label>(view);
        Assert.Equal("$NameTemperatureAndFruitLabel$", view.Text);

        // Act
        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal(" likes s when its 0 outside", view.Text);

        viewModel.Name = "Billy";

        Assert.Equal("Billy likes s when its 0 outside", view.Text);

        viewModel.SelectedFruit = Fruit.Apple;

        Assert.Equal("Billy likes Apples when its 0 outside", view.Text);

        viewModel.Temperature = 24.5;

        Assert.Equal("Billy likes Apples when its 24.5 outside", view.Text);
    }

    [Fact]
    public void SetTranslationMultiBindingView_WhenTranslationKeyExistsForCultureOneButNotCultureTwo_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        var view = new Label();

        var viewModel = new SomeViewModel()
        {
            Name = "Jimmy",
            SelectedFruit = Fruit.Cherry,
            Temperature = 19.2,
        };

        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} likes {1}s when its {2} outside" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        view.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [nameBinding, fruitBinding, temperatureBinding]);

        Assert.IsType<Label>(view);
        Assert.Equal("Jimmy likes Cherrys when its 19.2 outside", view.Text);

        // Act
        var frFrLocalization = new Localization(new CultureInfo("fr-Fr"));

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("$NameTemperatureAndFruitLabel$", view.Text);
    }

    [Fact]
    public void SetTranslationMultiBindingView_WhenTranslationKeyExistsForAllCultures_ShouldFormatTranslationCorrectly()
    {
        // Arrange
        var view = new Label();

        var viewModel = new SomeViewModel()
        {
            Name = "Jimmy",
            SelectedFruit = Fruit.Cherry,
            Temperature = 19.2,
        };

        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);
        var fruitBinding = new Binding(nameof(SomeViewModel.SelectedFruit), source: viewModel);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} likes {1}s when its {2} outside" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        view.SetTranslationMultiBinding(Label.TextProperty, "NameTemperatureAndFruitLabel", [nameBinding, fruitBinding, temperatureBinding]);

        Assert.IsType<Label>(view);
        Assert.Equal("Jimmy likes Cherrys when its 19.2 outside", view.Text);

        // Act
        var frFrLocalization = new Localization(new CultureInfo("fr-Fr"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "NameTemperatureAndFruitLabel", "{0} aime les {1} quand il est {2} dehors" }
            }
        };

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Assert
        Assert.Equal("Jimmy aime les Cherry quand il est 19.2 dehors", view.Text);
    }

    [Fact]
    public void SetTranslationMultiBindingView_WhenBindableObjectIsNull_ShouldThrow()
    {
        // Arrange
        var viewModel = new SomeViewModel();

        View view = null!;
        var temperatureBinding = new Binding(nameof(SomeViewModel.Temperature), source: viewModel);
        var nameBinding = new Binding(nameof(SomeViewModel.Name), source: viewModel);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => view.SetTranslationMultiBinding(Label.TextProperty, "NameAndTemperatureLabel", [temperatureBinding, nameBinding]));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'bindableObject')", ex.Message);
    }

    [Fact]
    public void SetTranslationMultiBindingView_WhenBindingIsNull_ShouldThrow()
    {
        // Arrange
        var view = new Label();

        // Act
        var ex = Assert.Throws<ArgumentException>(() => view.SetTranslationMultiBinding(Label.TextProperty, "NameAndTemperatureLabel", []));

        // Assert
        Assert.Equal("Required input Bindings was empty. (Parameter 'Bindings')", ex.Message);
    }

    #endregion -- View

    #endregion - SetTranslationMultiBinding

    #endregion Tests

    #region Test Data

    private sealed class UpperCaseConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string str
                ? str.ToUpper(CultureInfo.InvariantCulture)
                : null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class UpperCaseWithNumberConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is not int number)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return value is string str
                ? $"{str.ToUpper(CultureInfo.InvariantCulture)}_{number}"
                : null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    private sealed partial class SomeViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial double Temperature { get; set; }

        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Fruit? SelectedFruit { get; set; }
    }

    private enum Fruit
    {
        [Description("Fruit_Banana")]
        Banana,

        [Description("Fruit_Apple")]
        Apple,

        [Description("Fruit_Cherry")]
        Cherry
    }

    #endregion Test Data
}

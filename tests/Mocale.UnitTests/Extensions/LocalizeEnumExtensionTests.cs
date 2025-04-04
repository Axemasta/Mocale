using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Collections;
using Mocale.UnitTests.Fixtures;

namespace Mocale.UnitTests.Extensions;

[Collection(CollectionNames.MocaleLocatorTests)]
public partial class LocalizeEnumExtensionTests : FixtureBase<LocalizeEnumExtension>
{
    #region Setup

    private readonly IServiceProvider serviceProvider = Mock.Of<IServiceProvider>();
    private readonly MocaleConfiguration mocaleConfiguration = new();
    private readonly TranslatorManagerProxy translatorManager = new();

    public override LocalizeEnumExtension CreateSystemUnderTest()
    {
        return new LocalizeEnumExtension(translatorManager);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void Constructor_WhenParameterless_ShouldUseMocaleLocator()
    {
        // Arrange
        var translatorManagerMock = new Mock<ITranslatorManager>();
        var configurationMock = new Mock<IMocaleConfiguration>();

        MocaleLocator.SetInstance(translatorManagerMock.Object);
        MocaleLocator.MocaleConfiguration = configurationMock.Object;

        // Act
        var localizeExtension = new LocalizeEnumExtension();
        var extensionTranslatorManager = localizeExtension.GetTranslatorManager();

        // Assert
        Assert.Equal(translatorManagerMock.Object, extensionTranslatorManager);
    }

    [Fact]
    public void ProvideValue_WhenUsingDefault_ShouldCreateWithDefaults()
    {
        // Arrange
        var source = new Picker();
        Sut.Path = "Name";
        Sut.Source = source;

        // Act
        var multiBinding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("{0}", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(2, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal(nameof(translatorManager.CurrentCulture), bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal("Name", bindingTwo.Path);
        Assert.Equal(BindingMode.OneWay, bindingTwo.Mode);
        Assert.Null(bindingTwo.Converter);
        Assert.Null(bindingTwo.ConverterParameter);
        Assert.Equal(source, bindingTwo.Source);
    }

    [Fact]
    public void ProvideValue_WhenSettingProperties_ShouldCreateWithProperties()
    {
        // Arrange
        var converter = Mock.Of<IValueConverter>();
        var source = new Picker();

        Sut.Path = "Name";
        Sut.Mode = BindingMode.TwoWay;
        Sut.StringFormat = "!!{0}__";
        Sut.Converter = converter;
        Sut.ConverterParameter = 1357;
        Sut.Source = source;

        // Act
        var multiBinding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("!!{0}__", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(2, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal(nameof(translatorManager.CurrentCulture), bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal("Name", bindingTwo.Path);
        Assert.Equal(BindingMode.TwoWay, bindingTwo.Mode);
        // Converters will be executed after the binding to ensure correct order
        Assert.Null(bindingTwo.Converter);
        Assert.Null(bindingTwo.ConverterParameter);
        Assert.Equal(source, bindingTwo.Source);
    }

    [Fact]
    public void Convert_WhenThereArentTwoInputs_ShouldReturnEmptyString()
    {
        var noArgs = Sut.Convert([], typeof(Label), null!, CultureInfo.InvariantCulture);
        var oneArg = Sut.Convert([Vehicle.Train], typeof(Label), null!, CultureInfo.InvariantCulture);
        var argsThrice = Sut.Convert([Vehicle.Train, Vehicle.Bike, Vehicle.Bicycle], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, noArgs);
        Assert.Equal(string.Empty, oneArg);
        Assert.Equal(string.Empty, argsThrice);
    }

    [Fact]
    public void Convert_WhenInputTwoItNotNullOrEnum_ShouldThrow()
    {
        var ex = Assert.Throws<NotSupportedException>(() => Sut.Convert([new CultureInfo("en-GB"), "I am not an enum :)"], typeof(Label), null!, CultureInfo.InvariantCulture));

        Assert.Equal("Value must be of type Enum, instead value was of type String. Use LocalizeBinding to localize non enum values!", ex.Message);
    }

    [Fact]
    public void Convert_WhenInputTwoIsNullOrEnum_ShouldReturnEmptyString()
    {
        var translationKey = Sut.Convert([new CultureInfo("en-GB"), null!], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, translationKey);
    }

    [Fact]
    public void Convert_WhenConfiguredToNotUseAttributes_ShouldReturnEnumString()
    {
        // Arrange
        MocaleLocator.MocaleConfiguration = mocaleConfiguration;

        mocaleConfiguration.EnumBehavior = new LocalizeEnumBehavior
        {
            UseAttribute = false
        };

        // Act
        var translationKey = Sut.Convert([new CultureInfo("en-GB"), Vehicle.Train], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Train", translationKey);
    }

    [Fact]
    public void Convert_WhenConfiguredToUseAttributes_ShouldExtractFromConfiguredBehavior()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Key_Car",  "Car (English)" },
                { "Key_Lorry", "Lorry (English)" },
                { "Key_Van", "Van (English)" },
                { "Key_Bike", "Bike (English)" },
                { "Key_Bicycle", "Bicycle (English)" },
                { "Key_Train", "Train (English)" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        MocaleLocator.MocaleConfiguration = mocaleConfiguration;

        // Act
        var translationKey = Sut.Convert([new CultureInfo("en-GB"), Vehicle.Train], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Train (English)", translationKey);
    }

    [Fact]
    public void Convert_WhenConfiguredToUseAttributesWithSpecificRule_ShouldExtractFromSpecificRule()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Key_Proteins",  "Proteins" },
                { "Key_Carbohydrates", "Carbohydrates" },
                { "Key_FatsAndOils", "Fats And Oils" },
                { "Key_FruitsAndVegetables", "Fruits And Vegetables" },
                { "Key_Dairy", "Dairy" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        MocaleLocator.MocaleConfiguration = mocaleConfiguration;

        mocaleConfiguration.EnumBehavior = new LocalizeEnumBehavior
        {
            UseAttribute = true,
            OverrideRules =
            {
                {
                    typeof(FoodGroup), new LocalizeEnumRule()
                    {
                        UseAttribute = true,
                        AttributePropertyName = nameof(FoodGroupLocalizationAttribute.CustomTranslationKey),
                        LocalizeAttribute = typeof(FoodGroupLocalizationAttribute)
                    }
                }
            }
        };

        // Act
        var translationKey = Sut.Convert([new CultureInfo("en-GB"), FoodGroup.FruitsAndVegetables], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Fruits And Vegetables", translationKey);
    }

    [Fact]
    public void Convert_WhenConfiguredToNotUseAttributesButOverrideRuleExists_ShouldUseOverrideRuleOverGlobal()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Key_Proteins",  "Proteins" },
                { "Key_Carbohydrates", "Carbohydrates" },
                { "Key_FatsAndOils", "Fats And Oils" },
                { "Key_FruitsAndVegetables", "Fruits And Vegetables" },
                { "Key_Dairy", "Dairy" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        MocaleLocator.MocaleConfiguration = mocaleConfiguration;

        mocaleConfiguration.EnumBehavior = new LocalizeEnumBehavior
        {
            UseAttribute = false,
            OverrideRules =
            {
                {
                    typeof(FoodGroup), new LocalizeEnumRule()
                    {
                        UseAttribute = true,
                        AttributePropertyName = nameof(FoodGroupLocalizationAttribute.CustomTranslationKey),
                        LocalizeAttribute = typeof(FoodGroupLocalizationAttribute)
                    }
                }
            }
        };

        // Act
        var translationKey = Sut.Convert([new CultureInfo("en-GB"), FoodGroup.FruitsAndVegetables], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Fruits And Vegetables", translationKey);
    }

    [Fact]
    public void ConvertBack_ShouldNotBeImplemented()
    {
        Assert.Throws<NotImplementedException>(() => Sut.ConvertBack("Fruits And Vegetables", [typeof(Label)], null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void IntegrationTest()
    {
        MocaleLocator.MocaleConfiguration = mocaleConfiguration;
        _ = new ControlsFixtureBase();
        var label = new Label();

        var viewModel = new VehicleViewModel();

        Sut.Path = nameof(viewModel.SelectedVehicle);
        Sut.Source = viewModel;

        label.SetBinding(Label.TextProperty, Sut.ProvideValue(Mock.Of<IServiceProvider>()));

        Assert.Equal("", label.Text);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Key_Car",  "Car (English)" },
                { "Key_Lorry", "Lorry (English)" },
                { "Key_Van", "Van (English)" },
                { "Key_Bike", "Bike (English)" },
                { "Key_Bicycle", "Bicycle (English)" },
                { "Key_Train", "Train (English)" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        Assert.Equal("", label.Text);

        viewModel.SelectedVehicle = Vehicle.Bicycle;

        Assert.Equal("Bicycle (English)", label.Text);

        var frFRLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Key_Car",  "Voiture (Français)" },
                { "Key_Lorry", "Camion (Français)" },
                { "Key_Van", "Van (Français)" },
                { "Key_Bike", "Moto (Français)" },
                { "Key_Bicycle", "Vélo (Français)" },
                { "Key_Train", "Former (Français)" },
            }
        };

        translatorManager.UpdateTranslations(frFRLocalization, TranslationSource.Internal);

        Assert.Equal("Vélo (Français)", label.Text);

        viewModel.SelectedVehicle = Vehicle.Car;

        Assert.Equal("Voiture (Français)", label.Text);

        viewModel.SelectedVehicle = null;

        Assert.Equal("", label.Text);
    }

    [Fact]
    public void Convert_WhenConverterSet_ShouldConvertAfterTranslation()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Key_Car",  "Car (English)" },
                { "Key_Lorry", "Lorry (English)" },
                { "Key_Van", "Van (English)" },
                { "Key_Bike", "Bike (English)" },
                { "Key_Bicycle", "Bicycle (English)" },
                { "Key_Train", "Train (English)" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        MocaleLocator.MocaleConfiguration = mocaleConfiguration;

        Sut.Converter = new FlagConverter();

        // Act
        var translationKey = Sut.Convert([new CultureInfo("en-GB"), Vehicle.Train], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Train (🏴󠁧󠁢󠁥󠁮󠁧󠁿)", translationKey);
    }

    [Fact]
    public void Convert_WhenConverterAndParameterSet_ShouldConvertWithParameterAfterTranslation()
    {
        // Arrange
        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Key_Car",  "Car (English)" },
                { "Key_Lorry", "Lorry (English)" },
                { "Key_Van", "Van (English)" },
                { "Key_Bike", "Bike (English)" },
                { "Key_Bicycle", "Bicycle (English)" },
                { "Key_Train", "Train (English)" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        MocaleLocator.MocaleConfiguration = mocaleConfiguration;

        Sut.Converter = new MultipleFlagsConverter();
        Sut.ConverterParameter = 3;

        // Act
        var translationKey = Sut.Convert([new CultureInfo("en-GB"), Vehicle.Train], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Train (🏴󠁧󠁢󠁥󠁮󠁧󠁿🏴󠁧󠁢󠁥󠁮󠁧󠁿🏴󠁧󠁢󠁥󠁮󠁧󠁿)", translationKey);
    }

    #endregion Tests

    #region Test Data

    private enum Vehicle
    {
        [Description("Key_Car")] Car,

        [Description("Key_Lorry")] Lorry,

        [Description("Key_Van")] Van,

        [Description("Key_Bike")] Bike,

        [Description("Key_Bicycle")] Bicycle,

        [Description("Key_Train")] Train
    }

    private enum FoodGroup
    {
        [FoodGroupLocalization("Key_Proteins")]
        Proteins,

        [FoodGroupLocalization("Key_Carbohydrates")]
        Carbohydrates,

        [FoodGroupLocalization("Key_FatsAndOils")]
        FatsAndOils,

        [FoodGroupLocalization("Key_FruitsAndVegetables")]
        FruitsAndVegetables,

        [FoodGroupLocalization("Key_Dairy")] Dairy
    }

    [AttributeUsage(AttributeTargets.Field)]
    private sealed class FoodGroupLocalizationAttribute(string value) : Attribute
    {
        public string CustomTranslationKey { get; } = value;
    }

    private sealed partial class VehicleViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial Vehicle? SelectedVehicle { get; set; }
    }

    private class FlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string str)
            {
                throw new InvalidOperationException();
            }

            return str.Replace("English", "🏴󠁧󠁢󠁥󠁮󠁧󠁿");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    private class MultipleFlagsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string str)
            {
                throw new InvalidOperationException();
            }

            if (parameter is not int count)
            {
                throw new InvalidOperationException();
            }

            var flag = "🏴󠁧󠁢󠁥󠁮󠁧󠁿";
            var builder = new System.Text.StringBuilder(flag.Length * count);

            for (int i = 0; i < count; i++)
            {
                builder.Append(flag);
            }

            return str.Replace("English", builder.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Test Data
}

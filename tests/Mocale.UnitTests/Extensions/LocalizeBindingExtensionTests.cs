using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Collections;

namespace Mocale.UnitTests.Extensions;

[Collection(CollectionNames.MocaleLocatorTests)]
public partial class LocalizeBindingExtensionTests : FixtureBase<LocalizeBindingExtension>
{
    #region Setup

    private readonly IServiceProvider serviceProvider = Mock.Of<IServiceProvider>();
    private readonly TranslatorManagerProxy translatorManager = new();

    public override LocalizeBindingExtension CreateSystemUnderTest()
    {
        return new LocalizeBindingExtension(translatorManager);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void Constructor_WhenParameterless_ShouldUseMocaleLocator()
    {
        // Arrange
        var translatorManagerMock = new Mock<ITranslatorManager>();
        MocaleLocatorHelper.SetTranslatorManager(translatorManagerMock.Object);

        // Act
        var localizeExtension = new LocalizeBindingExtension();
        var extensionTranslatorManager = localizeExtension.GetTranslatorManager();

        // Assert
        Assert.Equal(translatorManagerMock.Object, extensionTranslatorManager);
    }

    [Fact]
    public void ProvideValue_WhenKeyAndKeyConverterAreNull_ShouldThrow()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = null;

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => Sut.ProvideValue(serviceProvider));

        // Assert
        Assert.Equal("Neither TranslationKey or KeyConverter were set. Use must set one of these, please see the documentation for details", ex.Message);
    }

    [Fact]
    public void ProvideValue_WhenTranslationKeyIsSet_ShouldCreateBinding()
    {
        // Arrange
        Sut.TranslationKey = "KeyOne";

        // Act
        var multiBinding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("{0}", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(2, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal("[KeyOne]", bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal(".", bindingTwo.Path);
        Assert.Equal(BindingMode.OneWay, bindingTwo.Mode);
        Assert.Null(bindingTwo.Converter);
        Assert.Null(bindingTwo.ConverterParameter);
        Assert.Null(bindingTwo.Source);
    }

    [Fact]
    public void ProvideValue_WhenTranslationKeyIsSetAndValuesAreSet_ShouldPassValuesToSecondBinding()
    {
        // Arrange
        var converter = Mock.Of<IValueConverter>();
        var source = new Picker();

        Sut.TranslationKey = "KeyOne";
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
        Assert.Equal("[KeyOne]", bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal("Name", bindingTwo.Path);
        Assert.Equal(BindingMode.TwoWay, bindingTwo.Mode);
        Assert.Equal(converter, bindingTwo.Converter);
        Assert.Equal(1357, bindingTwo.ConverterParameter);
        Assert.Equal(source, bindingTwo.Source);
    }

    [Fact]
    public void ProvideValue_WhenTranslationKeyIsSetShouldBehaveTheSame_WhenUsingIMarkupExtension()
    {
        // Arrange
        var converter = Mock.Of<IValueConverter>();
        var source = new Picker();

        Sut.TranslationKey = "KeyOne";
        Sut.Path = "Name";
        Sut.Mode = BindingMode.TwoWay;
        Sut.StringFormat = "!!{0}__";
        Sut.Converter = converter;
        Sut.ConverterParameter = 1357;
        Sut.Source = source;

        // Act
        var value = ((IMarkupExtension)Sut).ProvideValue(serviceProvider);

        // Assert
        var multiBinding = Assert.IsType<MultiBinding>(value);
        Assert.Equal("!!{0}__", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(2, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal("[KeyOne]", bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal("Name", bindingTwo.Path);
        Assert.Equal(BindingMode.TwoWay, bindingTwo.Mode);
        Assert.Equal(converter, bindingTwo.Converter);
        Assert.Equal(1357, bindingTwo.ConverterParameter);
        Assert.Equal(source, bindingTwo.Source);
    }

    [Fact]
    public void ConvertTranslationKey_WhenThereArentTwoInputs_ShouldReturnEmptyString()
    {
        Sut.TranslationKey = "Key";
        Sut.KeyConverter = null;

        var noArgs = Sut.Convert([], typeof(Label), null!, CultureInfo.InvariantCulture);
        var oneArg = Sut.Convert(["en-GB"], typeof(Label), null!, CultureInfo.InvariantCulture);
        var argsThrice = Sut.Convert(["sausage roll", "sausage roll", "sausage roll"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, noArgs);
        Assert.Equal(string.Empty, oneArg);
        Assert.Equal(string.Empty, argsThrice);
    }

    [Fact]
    public void ConvertTranslationKey_WhenFirstValueIsNotString_ShouldReturnEmpty()
    {
        Sut.TranslationKey = "Key";
        Sut.KeyConverter = null;

        var invalidInput = Sut.Convert([1234, "Name"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, invalidInput);
    }

    [Fact]
    public void ConvertTranslationKey_WhenTranslationIsNotFormatString_ShouldNotPlaceBindedValueInString()
    {
        Sut.TranslationKey = "Key";
        Sut.KeyConverter = null;

        // This test is to show that parameterization won't work when not working with parameterized strings!
        var formattedString = Sut.Convert(["I am not a format string", "Format me in your string!"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal("I am not a format string", formattedString);
    }

    [Fact]
    public void ConvertTranslationKey_WhenSecondValueIsString_ShouldFormatCorrectly()
    {
        Sut.TranslationKey = "Key";
        Sut.KeyConverter = null;

        var result = Sut.Convert(["Hello {0}", "Name"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal("Hello Name", result);
    }

    [Fact]
    public void ConvertTranslationKey_WhenSecondValueIsNull_ShouldNotThrow()
    {
        Sut.TranslationKey = "Key";
        Sut.KeyConverter = null;

        // When bindings first attach they will fire a pass null, inspite of what #nullable has to say about it!!!
        var result = Sut.Convert(["Hello '{0}'", null!], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal("Hello ''", result);
    }

    [Theory]
    [MemberData(nameof(SecondValueNotStringTestData))]
    public void ConvertTranslationKey_WhenSecondValueIsNotString_ShouldFormatCorrectly(string formatString, object value, string expectedValue, CultureInfo cultureInfo)
    {
        Sut.TranslationKey = "Key";
        Sut.KeyConverter = null;

        var result = Sut.Convert([formatString, value], typeof(Label), null!, cultureInfo);

        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void ConvertBack_ShouldNotBeImplemented()
    {
        Assert.Throws<NotImplementedException>(() => Sut.ConvertBack("Hello Name", [typeof(Label)], null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void IntegrationTest_ForTranslationKey()
    {
        _ = new ControlsFixtureBase();
        var label = new Label();

        var viewModel = new GreetingViewModel();

        Sut.TranslationKey = "GreetingMessage";
        Sut.Path = nameof(GreetingViewModel.Name);
        Sut.Source = viewModel;

        label.SetBinding(Label.TextProperty, Sut.ProvideValue(Mock.Of<IServiceProvider>()));

        Assert.Equal("$GreetingMessage$", label.Text);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "GreetingMessage",  "Hello {0}" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        Assert.Equal("Hello ", label.Text);

        viewModel.Name = "Sir Dotsworth";

        Assert.Equal("Hello Sir Dotsworth", label.Text);

        var frFRLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "GreetingMessage",  "Bonjour {0}" },
            }
        };

        translatorManager.UpdateTranslations(frFRLocalization, TranslationSource.Internal);

        Assert.Equal("Bonjour Sir Dotsworth", label.Text);

        viewModel.Name = "Alex";

        Assert.Equal("Bonjour Alex", label.Text);
    }

    [Fact]
    public void ProvideValue_WhenKeyConverterIsSet_ShouldCreateBinding()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();

        // Act
        var multiBinding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("{0}", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(2, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal("CurrentCulture", bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal(".", bindingTwo.Path);
        Assert.Equal(BindingMode.OneWay, bindingTwo.Mode);
        Assert.Null(bindingTwo.Converter);
        Assert.Null(bindingTwo.ConverterParameter);
        Assert.Null(bindingTwo.Source);
    }

    [Fact]
    public void ProvideValue_WhenKeyConverterIsSetShouldBehaveTheSame_WhenUsingIMarkupExtension()
    {
        // Arrange
        var converter = Mock.Of<IValueConverter>();
        var source = new Picker();

        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();
        Sut.Path = "Name";
        Sut.Mode = BindingMode.TwoWay;
        Sut.StringFormat = "!!{0}__";
        Sut.Converter = converter;
        Sut.ConverterParameter = 1357;
        Sut.Source = source;

        // Act
        var value = ((IMarkupExtension)Sut).ProvideValue(serviceProvider);

        // Assert
        var multiBinding = Assert.IsType<MultiBinding>(value);
        Assert.Equal("!!{0}__", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(2, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal("CurrentCulture", bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal("Name", bindingTwo.Path);
        Assert.Equal(BindingMode.TwoWay, bindingTwo.Mode);
        Assert.Null(bindingTwo.Converter);
        Assert.Null(bindingTwo.ConverterParameter);
        Assert.Equal(source, bindingTwo.Source);
    }

    [Fact]
    public void ConvertKeyConverter_WhenKeyConverterIsNull_ShouldThgrow()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = null;

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => Sut.ConvertKeyConverterBinding(
            [
                "en-GB",
                new ChilliInfo()
                {
                    Name = "Bell Pepper", Origin = "The Americas", Scovilles = new ScovilleHeatUnits(0, 0)
                }
            ],
            typeof(Label), new CultureInfo("en-GB")));

        Assert.Equal("Key converter was null, unable to convert binding.", ex.Message);
    }

    [Fact]
    public void ConvertKeyConverter_WhenThereArentTwoInputs_ShouldReturnEmptyString()
    {
        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();

        var noArgs = Sut.Convert([], typeof(Label), null!, CultureInfo.InvariantCulture);
        var oneArg = Sut.Convert(["en-GB"], typeof(Label), null!, CultureInfo.InvariantCulture);
        var argsThrice = Sut.Convert(["sausage roll", "sausage roll", "sausage roll"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, noArgs);
        Assert.Equal(string.Empty, oneArg);
        Assert.Equal(string.Empty, argsThrice);
    }

    [Fact]
    public void ConvertKeyConverter_WhenKeyConverterDoesntReturnPopulatedString_ShouldReturnEmptyString()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = new EmptyKeyConverter();

        var bellPepper = new ChilliInfo()
        {
            Name = "Bell Pepper",
            Origin = "The Americas",
            Scovilles = new ScovilleHeatUnits(0, 0)
        };

        // Act
        var result = Sut.Convert(["en-GB", bellPepper], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ConvertKeyConverter_WhenKeyConverterProvidesKeyAndConverterIsNull_ShouldTranslateAndReturn()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "BellPepperKey", "Bell Pepper" },
                { "JalapenoKey", "Jalapeno" },
                { "BirdsEyeKey", "Bird's Eye" },
                { "HabaneroKey", "Habanero" },
                { "CarolinaReaperKey", "Carolina Reaper" }
            }
        }, TranslationSource.Internal);

        var bellPepper = new ChilliInfo()
        {
            Name = "Bell Pepper",
            Origin = "The Americas",
            Scovilles = new ScovilleHeatUnits(0, 0)
        };

        // Act
        var result = Sut.Convert(["en-GB", bellPepper], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Bell Pepper", result);
    }

    [Fact]
    public void ConvertKeyConverter_WhenKeyConverterProvidesKeyWithKeyParameter_ShouldTranslateAndReturn()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();
        Sut.KeyConverterParameter = true;

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "BellPepperKey", "Bell Pepper" },
                { "JalapenoKey", "Jalapeno" },
                { "BirdsEyeKey", "Bird's Eye" },
                { "HabaneroKey", "Habanero" },
                { "CarolinaReaperKey", "Carolina Reaper" },
                { "BellPepperEmojiKey", "Bell Pepper 🫑" },
                { "JalapenoEmojiKey", "Jalapeno 🌶️" },
                { "BirdsEyeEmojiKey", "Bird's Eye 🕊️👁️" },
                { "HabaneroEmojiKey", "Habanero 🔥" },
                { "CarolinaReaperEmojiKey", "Carolina Reaper 🔥☠️" }
            }
        }, TranslationSource.Internal);

        var bellPepper = new ChilliInfo()
        {
            Name = "Bell Pepper",
            Origin = "The Americas",
            Scovilles = new ScovilleHeatUnits(0, 0)
        };

        // Act
        var result = Sut.Convert(["en-GB", bellPepper], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Bell Pepper 🫑", result);
    }

    [Fact]
    public void ConvertKeyConverter_WhenKeyConverterProvidesKeyAndConverterIsSet_ShouldTranslateApplyConverter()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();
        Sut.Converter = new UpperCaseRepeatConverter();

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "BellPepperKey", "Bell Pepper" },
                { "JalapenoKey", "Jalapeno" },
                { "BirdsEyeKey", "Bird's Eye" },
                { "HabaneroKey", "Habanero" },
                { "CarolinaReaperKey", "Carolina Reaper" }
            }
        }, TranslationSource.Internal);

        var bellPepper = new ChilliInfo()
        {
            Name = "Bell Pepper",
            Origin = "The Americas",
            Scovilles = new ScovilleHeatUnits(0, 0)
        };

        // Act
        var result = Sut.Convert(["en-GB", bellPepper], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("BELL PEPPER", result);
    }

    [Fact]
    public void ConvertKeyConverter_WhenKeyConverterProvidesKeyAndConverterIsSetWithParameter_ShouldTranslateApplyConverter()
    {
        // Arrange
        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();
        Sut.Converter = new UpperCaseRepeatConverter();
        Sut.ConverterParameter = 3;

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "BellPepperKey", "Bell Pepper" },
                { "JalapenoKey", "Jalapeno" },
                { "BirdsEyeKey", "Bird's Eye" },
                { "HabaneroKey", "Habanero" },
                { "CarolinaReaperKey", "Carolina Reaper" }
            }
        }, TranslationSource.Internal);

        var bellPepper = new ChilliInfo()
        {
            Name = "Bell Pepper",
            Origin = "The Americas",
            Scovilles = new ScovilleHeatUnits(0, 0)
        };

        // Act
        var result = Sut.Convert(["en-GB", bellPepper], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("BELL PEPPERBELL PEPPERBELL PEPPER", result);
    }

    [Fact]
    public void IntegrationTest_ForKeyConverter()
    {
        _ = new ControlsFixtureBase();
        var label = new Label();

        var viewModel = new ChilliViewModel();

        Sut.TranslationKey = null;
        Sut.KeyConverter = new ChilliKeyConverter();
        Sut.KeyConverterParameter = true;
        Sut.Path = nameof(ChilliViewModel.SelectedChilli);
        Sut.Source = viewModel;

        label.SetBinding(Label.TextProperty, Sut.ProvideValue(Mock.Of<IServiceProvider>()));

        Assert.Equal("", label.Text);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "BellPepperKey", "Bell Pepper" },
                { "JalapenoKey", "Jalapeno" },
                { "BirdsEyeKey", "Bird's Eye" },
                { "HabaneroKey", "Habanero" },
                { "CarolinaReaperKey", "Carolina Reaper" },
                { "BellPepperEmojiKey", "Bell Pepper 🫑" },
                { "JalapenoEmojiKey", "Jalapeno 🌶️" },
                { "BirdsEyeEmojiKey", "Bird's Eye 🕊️👁️" },
                { "HabaneroEmojiKey", "Habanero 🔥" },
                { "CarolinaReaperEmojiKey", "Carolina Reaper 🔥☠️" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        Assert.Equal("", label.Text);

        viewModel.SelectedChilli = viewModel.Chillis[2];

        Assert.Equal("Bird's Eye 🕊️👁️", label.Text);

        var frFRLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "BellPepperKey", "Poivron" },
                { "JalapenoKey", "Jalapeño" },
                { "BirdsEyeKey", "Piment oiseau" },
                { "HabaneroKey", "Habanero" },
                { "CarolinaReaperKey", "Carolina Reaper" },
                { "BellPepperEmojiKey", "Poivron 🫑" },
                { "JalapenoEmojiKey", "Jalapeño 🌶️" },
                { "BirdsEyeEmojiKey", "Piment oiseau 🕊️👁️" },
                { "HabaneroEmojiKey", "Habanero 🔥" },
                { "CarolinaReaperEmojiKey", "Carolina Reaper 🔥☠️" }
            }
        };

        translatorManager.UpdateTranslations(frFRLocalization, TranslationSource.Internal);

        Assert.Equal("Piment oiseau 🕊️👁️", label.Text);

        viewModel.SelectedChilli = viewModel.Chillis[3];

        Assert.Equal("Habanero 🔥", label.Text);
    }

    #endregion Tests

    #region Test Data

    public static TheoryData<string, object, string, CultureInfo> SecondValueNotStringTestData => new()
    {
        { "The temperature is {0}\u00b0C", 28.3d, "The temperature is 28.3\u00b0C", new CultureInfo("en-GB") },
        { "La température est {0}\u00b0C", 28.3d, "La température est 28.3\u00b0C", new CultureInfo("fr-FR") }
    };

    private sealed partial class GreetingViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;
    }

    private sealed record ScovilleHeatUnits(int LowerBound, int UpperBound);

    private sealed partial class ChilliInfo : ObservableObject
    {
        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Origin { get; set; } = string.Empty;

        [ObservableProperty]
        public required partial ScovilleHeatUnits Scovilles { get; set; }
    }

    private sealed partial class ChilliViewModel : ObservableObject
    {
        public ObservableCollection<ChilliInfo> Chillis { get; } =
        [
            new ChilliInfo()
            {
                Name = "Bell Pepper",
                Origin = "The Americas",
                Scovilles = new ScovilleHeatUnits(0, 0)
            },
            new ChilliInfo()
            {
                Name = "Jalapeno pepper",
                Origin = "Native To Mexico",
                Scovilles = new ScovilleHeatUnits(2500, 10_000)
            },
            new ChilliInfo()
            {
                Name = "Bird's Eye Chilli",
                Origin = "Native To Mexico",
                Scovilles = new ScovilleHeatUnits(50_000, 100_000)
            },
            new ChilliInfo()
            {
                Name = "Habanero Chilli",
                Origin = "The Amazon",
                Scovilles = new ScovilleHeatUnits(100_000, 350_000)
            },
            new ChilliInfo()
            {
                Name = "Carolina Reaper",
                Origin = "Fort Mill, South Carolina, USA",
                Scovilles = new ScovilleHeatUnits(1_500_000, 2_500_000)
            }
        ];

        [ObservableProperty]
        public partial ChilliInfo? SelectedChilli { get; set; }
    }

    private sealed class ChilliKeyConverter : IKeyConverter
    {
        public string? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not ChilliInfo chilliInfo)
            {
                return null;
            }

            if (parameter is not bool useEmoji)
            {
                return chilliInfo.Name switch
                {
                    "Bell Pepper" => "BellPepperKey",
                    "Jalapeno pepper" => "JalapenoKey",
                    "Bird's Eye Chilli" => "BirdsEyeKey",
                    "Habanero Chilli" => "HabaneroKey",
                    "Carolina Reaper" => "CarolinaReaperKey",
                    _ => null
                };
            }

            return chilliInfo.Name switch
            {
                "Bell Pepper" => useEmoji ? "BellPepperEmojiKey" : "BellPepperKey",
                "Jalapeno pepper" => useEmoji ? "JalapenoEmojiKey" : "JalapenoKey",
                "Bird's Eye Chilli" => useEmoji ? "BirdsEyeEmojiKey" : "BirdsEyeKey",
                "Habanero Chilli" => useEmoji ? "HabaneroEmojiKey" : "HabaneroKey",
                "Carolina Reaper" => useEmoji ? "CarolinaReaperEmojiKey" : "CarolinaReaperKey",
                _ => null
            };
        }
    }

    private sealed class EmptyKeyConverter : IKeyConverter
    {
        public string? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }

    private sealed class UpperCaseRepeatConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                var upperCase = str.ToUpperInvariant();

                if (parameter is int repeats)
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < repeats; i++)
                    {
                        sb.Append(upperCase);
                    }
                    return sb.ToString();
                }
                else
                {
                    return upperCase;
                }
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Test Data
}

using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Fixtures;
using Xunit.Sdk;

namespace Mocale.UnitTests.Extensions;

public partial class LocalizeMultiBindingExtensionTests : FixtureBase<LocalizeBindingExtension>
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
        MocaleLocator.SetInstance(translatorManagerMock.Object);

        // Act
        var localizeExtension = new LocalizeBindingExtension();
        var extensionTranslatorManager = localizeExtension.GetTranslatorManager();

        // Assert
        Assert.Equal(translatorManagerMock.Object, extensionTranslatorManager);
    }

    [Fact]
    public void ProvideValue_WhenKeyIsNull_ShouldThrow()
    {
        // Arrange

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => Sut.ProvideValue(serviceProvider));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'TranslationKey')", ex.Message);
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
    public void ProvideValue_WhenValuesAreSet_ShouldPassValuesToSecondBinding()
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
    public void ProvideValue_ShouldBehaveTheSame_WhenUsingIMarkupExtension()
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
    public void Convert_WhenThereArentTwoInputs_ShouldReturnEmptyString()
    {
        var noArgs = Sut.Convert([], typeof(Label), null!, CultureInfo.InvariantCulture);
        var oneArg = Sut.Convert(["en-GB"], typeof(Label), null!, CultureInfo.InvariantCulture);
        var argsThrice = Sut.Convert(["sausage roll", "sausage roll", "sausage roll"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, noArgs);
        Assert.Equal(string.Empty, oneArg);
        Assert.Equal(string.Empty, argsThrice);
    }

    [Fact]
    public void Convert_WhenFirstValueIsNotString_ShouldReturnEmpty()
    {
        var invalidInput = Sut.Convert([1234, "Name"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, invalidInput);
    }

    [Fact]
    public void Convert_WhenTranslationIsNotFormatString_ShouldNotPlaceBindedValueInString()
    {
        // This test is to show that parameterization won't work when not working with parameterized strings!
        var formattedString = Sut.Convert(["I am not a format string", "Format me in your string!"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal("I am not a format string", formattedString);
    }

    [Fact]
    public void Convert_WhenSecondValueIsString_ShouldFormatCorrectly()
    {
        var result = Sut.Convert(["Hello {0}", "Name"], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal("Hello Name", result);
    }

    [Fact]
    public void Convert_WhenSecondValueIsNull_ShouldNotThrow()
    {
        // When bindings first attach they will fire a pass null, inspite of what #nullable has to say about it!!!
        var result = Sut.Convert(["Hello '{0}'", null!], typeof(Label), null!, CultureInfo.InvariantCulture);

        Assert.Equal("Hello ''", result);
    }

    [Theory]
    [MemberData(nameof(SecondValueNotStringTestData))]
    public void Convert_WhenSecondValueIsNotString_ShouldFormatCorrectly(string formatString, object value, string expectedValue, CultureInfo cultureInfo)
    {
        var result = Sut.Convert([formatString, value], typeof(Label), null!, cultureInfo);

        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void ConvertBack_ShouldNotBeImplemented()
    {
        Assert.Throws<NotImplementedException>(() => Sut.ConvertBack("Hello Name", [typeof(Label)], null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void IntegrationTest()
    {
        _ = new ControlsFixtureBase();
        var label = new Label();

        var viewModel = new GreetingViewModel();

        Sut.TranslationKey = "GreetingMessage";
        Sut.Path = nameof(GreetingViewModel.Name);
        Sut.Source = viewModel;

        label.SetBinding(Label.TextProperty, Sut.ProvideValue(Mock.Of<IServiceProvider>()));

        Assert.Equal("$GreetingMessage$", label.Text);

        var enGbLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "GreetingMessage",  "Hello {0}" },
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        Assert.Equal("Hello ", label.Text);

        viewModel.Name = "Sir Dotsworth";

        Assert.Equal("Hello Sir Dotsworth", label.Text);

        var frFRLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("fr-FR"),
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

    #endregion Tests

    #region Test Data

    public static TheoryData<string, object, string, CultureInfo> SecondValueNotStringTestData => new()
    {
        { "The temperature is {0}\u00b0C", 28.3d, "The temperature is 28.3\u00b0C", new CultureInfo("en-GB") },
        { "La température est {0}\u00b0C", 28.3d, "La température est 28.3\u00b0C", new CultureInfo("fr-FR") }
    };

    private partial class GreetingViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Name { get; set; }
    }

    #endregion Test Data
}

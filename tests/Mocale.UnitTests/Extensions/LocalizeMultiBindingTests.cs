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
public partial class LocalizeMultiBindingTests : FixtureBase<LocalizeMultiBindingExtension>
{
    #region Setup

    private readonly IServiceProvider serviceProvider = Mock.Of<IServiceProvider>();
    private readonly TranslatorManagerProxy translatorManager = new();

    public override LocalizeMultiBindingExtension CreateSystemUnderTest()
    {
        return new LocalizeMultiBindingExtension(translatorManager);
    }

    public LocalizeMultiBindingTests()
    {
        // Prevent test flakiness on GitHub actions
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
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
        var localizeExtension = new LocalizeMultiBindingExtension();
        var extensionTranslatorManager = localizeExtension.GetTranslatorManager();

        // Assert
        Assert.Equal(translatorManagerMock.Object, extensionTranslatorManager);
    }

    [Fact]
    public void ProvideValue_WhenTranslationKeyIsNull_ShouldThrow()
    {
        // Arrange

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => Sut.ProvideValue(serviceProvider));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'TranslationKey')", ex.Message);
    }

    [Fact]
    public void ProvideValue_WhenBindingsIsEmpty_ShouldThrow()
    {
        // Arrange
        Sut.TranslationKey = "MultiKey";

        // Act
        var ex = Assert.Throws<ArgumentException>(() => Sut.ProvideValue(serviceProvider));

        // Assert
        Assert.Equal("Required input Bindings was empty. (Parameter 'Bindings')", ex.Message);
    }

    [Fact]
    public void ProvideValue_WhenTranslationKeyAndBindingsAreSet_ShouldCreateMultiBinding()
    {
        // Arrange
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.0d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);

        // Act
        var multiBinding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("{0}", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(4, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal("[WelcomeMessage]", bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal("Name", bindingTwo.Path);
        Assert.Equal(BindingMode.Default, bindingTwo.Mode);
        Assert.Null(bindingTwo.Converter);
        Assert.Null(bindingTwo.ConverterParameter);
        Assert.Equal(viewModel, bindingTwo.Source);

        var bindingThree = Assert.IsType<Binding>(multiBinding.Bindings[2]);
        Assert.Equal("Date", bindingThree.Path);
        Assert.Equal(BindingMode.Default, bindingThree.Mode);
        Assert.Null(bindingThree.Converter);
        Assert.Null(bindingThree.ConverterParameter);
        Assert.Equal(viewModel, bindingThree.Source);

        var bindingFour = Assert.IsType<Binding>(multiBinding.Bindings[3]);
        Assert.Equal("Temperature", bindingFour.Path);
        Assert.Equal(BindingMode.Default, bindingFour.Mode);
        Assert.Null(bindingFour.Converter);
        Assert.Null(bindingFour.ConverterParameter);
        Assert.Equal(viewModel, bindingFour.Source);
    }

    [Fact]
    public void ProvideValue_WhenStringFormatIsSet_ShouldCreateMultiBindingWithFormat()
    {
        // Arrange
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.0d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);
        Sut.StringFormat = "{0:N}";

        // Act
        var multiBinding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("{0:N}", multiBinding.StringFormat);
        Assert.Equal(Sut, multiBinding.Converter);
        Assert.Equal(BindingMode.OneWay, multiBinding.Mode);
        Assert.Equal(4, multiBinding.Bindings.Count);

        var bindingOne = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        Assert.Equal("[WelcomeMessage]", bindingOne.Path);
        Assert.Equal(BindingMode.OneWay, bindingOne.Mode);
        Assert.Equal(translatorManager, bindingOne.Source);

        var bindingTwo = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal("Name", bindingTwo.Path);
        Assert.Equal(BindingMode.Default, bindingTwo.Mode);
        Assert.Null(bindingTwo.Converter);
        Assert.Null(bindingTwo.ConverterParameter);
        Assert.Equal(viewModel, bindingTwo.Source);

        var bindingThree = Assert.IsType<Binding>(multiBinding.Bindings[2]);
        Assert.Equal("Date", bindingThree.Path);
        Assert.Equal(BindingMode.Default, bindingThree.Mode);
        Assert.Null(bindingThree.Converter);
        Assert.Null(bindingThree.ConverterParameter);
        Assert.Equal(viewModel, bindingThree.Source);

        var bindingFour = Assert.IsType<Binding>(multiBinding.Bindings[3]);
        Assert.Equal("Temperature", bindingFour.Path);
        Assert.Equal(BindingMode.Default, bindingFour.Mode);
        Assert.Null(bindingFour.Converter);
        Assert.Null(bindingFour.ConverterParameter);
        Assert.Equal(viewModel, bindingFour.Source);
    }

    [Fact]
    public void Convert_WhenInputsDontMatchBindingsPlusOne_ShouldReturnEmptyString()
    {
        // Each binding will pass a value, plus 1 more for the translator manager
        // Arrange
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.0d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);

        // Act
        var noArgs = Sut.Convert([], typeof(Label), null!, CultureInfo.InvariantCulture);
        var bindingsOnly = Sut.Convert(["Gary", DateTime.Now, 10.0d], typeof(Label), null!, CultureInfo.InvariantCulture);
        var extraArgs = Sut.Convert(["Hello {0}, The date is {1} and the temperature is {2}\u00b0C", "Gary", DateTime.Now, 10.0d, "Extra arg!"], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(string.Empty, noArgs);
        Assert.Equal(string.Empty, bindingsOnly);
        Assert.Equal(string.Empty, extraArgs);
    }

    [Fact]
    public void Convert_WhenFirstArgumentIsNull_ShouldReturnEmptyString()
    {
        // Each binding will pass a value, plus 1 more for the translator manager
        // Arrange
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.0d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);

        // Act
        var translation = Sut.Convert([null!, "Gary", DateTime.Now, 10.0d], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(string.Empty, translation);
    }

    [Fact]
    public void Convert_WhenFirstArgumentIsNotString_ShouldThrow()
    {
        // Each binding will pass a value, plus 1 more for the translator manager
        // Arrange
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.0d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => Sut.Convert([17349, "Gary", DateTime.Now, 10.0d], typeof(Label), null!, CultureInfo.InvariantCulture));

        // Assert
        Assert.Equal($"The first value was not a string, actual type: Int32, if this method has been automatically called by Mocale please raise an issue on GitHub!", ex.Message);
    }

    [Fact]
    public void Convert_ShouldUseAllBindings_AndFormatCorrectly()
    {
        // Each binding will pass a value, plus 1 more for the translator manager
        // Arrange
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.3d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);

        // Act
        var translation = Sut.Convert(["Hello {0}, The date is {1} and the temperature is {2}\u00b0C", viewModel.Name, viewModel.Date, viewModel.Temperature], typeof(Label), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("Hello Gary, The date is 02/03/2025 13:42:23 and the temperature is 10.3\u00b0C", translation);
    }

    [Fact]
    public void Convert_ShouldToStringBindedValues_ToEnsureCorrectFormatting()
    {
        // Each binding will pass a value, plus 1 more for the translator manager
        // Arrange
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.3d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);

        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "WelcomeMessage", "Bonjour {0}, La date est le {1} et la température est le {2}\u00b0C" }
            }
        };

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        // Act
        var translation = Sut.Convert(["Bonjour {0}, La date est le {1} et la température est le {2}\u00b0C", viewModel.Name, viewModel.Date, viewModel.Temperature], typeof(Label), null!, new CultureInfo("fr-FR"));

        // Assert
        Assert.Equal("Bonjour Gary, La date est le 02/03/2025 13:42:23 et la température est le 10.3\u00b0C", translation);
    }

    [Fact]
    public void ConvertBack_ShouldNotBeImplemented()
    {
        Assert.Throws<NotImplementedException>(() => Sut.ConvertBack("Hello Gary, The date is 02/03/2015 and the temperature is 10\u00b0C", [typeof(Label)], null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void IntegrationTest()
    {
        // Arrange
        _ = new ControlsFixtureBase();
        var viewModel = new GreetingsViewModel()
        {
            Name = "Gary",
            Date = new DateTime(2025, 3, 2, 13, 42, 23),
            Temperature = 10.3d
        };

        var nameBinding = new Binding(nameof(GreetingsViewModel.Name), source: viewModel);
        var dateBinding = new Binding(nameof(GreetingsViewModel.Date), source: viewModel);
        var temperatureBinding = new Binding(nameof(GreetingsViewModel.Temperature), source: viewModel);

        Sut.TranslationKey = "WelcomeMessage";
        Sut.Bindings.Add(nameBinding);
        Sut.Bindings.Add(dateBinding);
        Sut.Bindings.Add(temperatureBinding);

        var enGbLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "WelcomeMessage", "Hello {0}, The date is {1} and the temperature is {2}\u00b0C" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        var label = new Label();

        // Act & Assert
        // Set initial binding => text should get set correctly
        label.SetBinding(Label.TextProperty, Sut.ProvideValue(Mock.Of<IServiceProvider>()));

        Assert.Equal("Hello Gary, The date is 02/03/2025 13:42:23 and the temperature is 10.3\u00b0C", label.Text);

        // Binded value changes => text should update
        viewModel.Name = "Helen";

        Assert.Equal("Hello Helen, The date is 02/03/2025 13:42:23 and the temperature is 10.3\u00b0C", label.Text);

        // Language changes => text should update
        var frFrLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "WelcomeMessage", "Bonjour {0}, La date est le {1} et la température est le {2}\u00b0C" }
            }
        };

        translatorManager.UpdateTranslations(frFrLocalization, TranslationSource.Internal);

        Assert.Equal("Bonjour Helen, La date est le 02/03/2025 13:42:23 et la température est le 10.3\u00b0C", label.Text);

        // Another property changes => text should update

        viewModel.Temperature = 29.7;

        Assert.Equal("Bonjour Helen, La date est le 02/03/2025 13:42:23 et la température est le 29.7\u00b0C", label.Text);
    }

    #endregion Tests

    #region Test Data

    private sealed partial class GreetingsViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public partial double Temperature { get; set; }

        [ObservableProperty]
        public partial DateTime Date { get; set; }
    }

    #endregion Test Data
}

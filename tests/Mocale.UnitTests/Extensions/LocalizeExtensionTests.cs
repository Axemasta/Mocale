using System.Globalization;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Fixtures;

namespace Mocale.UnitTests.Extensions;

public class LocalizeExtensionTests : FixtureBase<LocalizeExtension>
{
    #region Setup

    private readonly IServiceProvider serviceProvider = Mock.Of<IServiceProvider>();
    private readonly TranslatorManagerProxy translatorManager = new();

    public override LocalizeExtension CreateSystemUnderTest()
    {
        return new LocalizeExtension(translatorManager);
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
        var localizeExtension = new LocalizeExtension();
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
        Assert.Equal("Value cannot be null. (Parameter 'Key')", ex.Message);
    }

    [Fact]
    public void ProvideValue_WhenKeyIsSet_ShouldCreateBinding()
    {
        // Arrange
        Sut.Key = "KeyOne";

        // Act
        var binding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("[KeyOne]", binding.Path);
        Assert.Equal(BindingMode.OneWay, binding.Mode);
        Assert.Equal(translatorManager, binding.Source);
        Assert.Null(binding.Converter);
    }

    [Fact]
    public void ProvideValue_WhenConverterIsSet_ShouldPassConverter()
    {
        // Arrange
        var converter = Mock.Of<IValueConverter>();

        Sut.Key = "KeyOne";
        Sut.Converter = converter;

        // Act
        var binding = Sut.ProvideValue(serviceProvider);

        // Assert
        Assert.Equal("[KeyOne]", binding.Path);
        Assert.Equal(BindingMode.OneWay, binding.Mode);
        Assert.Equal(translatorManager, binding.Source);
        Assert.Equal(converter, binding.Converter);
    }

    [Fact]
    public void ProvideValue_ShouldBehaveTheSame_WhenUsingIMarkupExtension()
    {
        // Arrange
        var converter = Mock.Of<IValueConverter>();

        Sut.Key = "KeyOne";
        Sut.Converter = converter;

        // Act
        var bindingBase = ((IMarkupExtension)Sut).ProvideValue(serviceProvider);

        // Assert
        var binding = Assert.IsType<Binding>(bindingBase);
        Assert.Equal("[KeyOne]", binding.Path);
        Assert.Equal(BindingMode.OneWay, binding.Mode);
        Assert.Equal(translatorManager, binding.Source);
        Assert.Equal(converter, binding.Converter);
    }

    [Fact]
    public void IntegrationTest()
    {
        _ = new ControlsFixtureBase();
        var label = new Label();

        Sut.Key = "IntegrationTestMessage";

        label.SetBinding(Label.TextProperty, Sut.ProvideValue(Mock.Of<IServiceProvider>()));

        Assert.Equal("$IntegrationTestMessage$", label.Text);

        var enGbLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "IntegrationTestMessage", "This is my localization running in an integration test" }
            }
        };

        translatorManager.UpdateTranslations(enGbLocalization, TranslationSource.Internal);

        Assert.Equal("This is my localization running in an integration test", label.Text);

        var frFRLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("fr-FR"),
            Translations = new Dictionary<string, string>()
            {
                { "IntegrationTestMessage", "Ceci est ma localisation en cours d'exécution dans un test d'intégration" }
            }
        };

        translatorManager.UpdateTranslations(frFRLocalization, TranslationSource.Internal);

        Assert.Equal("Ceci est ma localisation en cours d'exécution dans un test d'intégration", label.Text);
    }

    #endregion Tests
}

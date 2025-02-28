using Mocale.Abstractions;
using Mocale.Extensions;
using Mocale.Testing;

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
        var bindingBase = Sut.ProvideValue(serviceProvider);

        // Assert
        var binding = Assert.IsType<Binding>(bindingBase);
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
        var bindingBase = Sut.ProvideValue(serviceProvider);

        // Assert
        var binding = Assert.IsType<Binding>(bindingBase);
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

    #endregion Tests
}

using Mocale.Abstractions;
using Mocale.Extensions;
using Mocale.Testing;
using Xunit.Sdk;

namespace Mocale.UnitTests.Extensions;

public class LocalizeMultiBindingExtensionTests : FixtureBase<LocalizeBindingExtension>
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

    #endregion Tests
}

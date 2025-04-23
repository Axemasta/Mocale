using Mocale.Abstractions;
using Mocale.Testing;
using Mocale.UnitTests.Collections;
using Mocale.UnitTests.Fixtures;

namespace Mocale.UnitTests.Testing;

[Collection(CollectionNames.TestingTests)]
public class MocaleLocatorHelperTests : MocaleLocatorFixture
{
    public MocaleLocatorHelperTests()
    {
        MocaleLocator.MocaleConfiguration = null;
        MocaleLocator.TranslatorManager = null;
    }

    [Fact]
    public void SetTranslatorManager()
    {
        // Arrange
        Assert.Null(MocaleLocator.TranslatorManager);

        var mockTranslatorManager = Mock.Of<ITranslatorManager>();

        // Act
        MocaleLocatorHelper.SetTranslatorManager(mockTranslatorManager);

        // Assert
        Assert.Equal(mockTranslatorManager, MocaleLocator.TranslatorManager);
    }

    [Fact]
    public void SetMocaleConfiguration()
    {
        // Arrange
        Assert.Null(MocaleLocator.MocaleConfiguration);

        var mockTranslatorManager = Mock.Of<IMocaleConfiguration>();

        // Act
        MocaleLocatorHelper.SetMocaleConfiguration(mockTranslatorManager);

        // Assert
        Assert.Equal(mockTranslatorManager, MocaleLocator.MocaleConfiguration);
    }
}

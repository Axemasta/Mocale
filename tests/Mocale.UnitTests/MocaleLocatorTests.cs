using Mocale.Abstractions;

namespace Mocale.UnitTests;

public class MocaleLocatorTests
{
    [Fact]
    public void SetInstance_Should_SetInstance()
    {
        // Arrange
        var translatorManager = new Mock<ITranslatorManager>();

        // Act
        MocaleLocator.SetInstance(translatorManager.Object);

        // Assert
        Assert.Equal(translatorManager.Object, MocaleLocator.TranslatorManager);
    }
}

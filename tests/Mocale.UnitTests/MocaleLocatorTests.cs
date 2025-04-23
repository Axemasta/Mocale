using Mocale.Abstractions;
using Mocale.Testing;

namespace Mocale.UnitTests;

public class MocaleLocatorTests
{
    [Fact]
    public void SetInstance_Should_SetInstance()
    {
        // Arrange
        var translatorManager = new Mock<ITranslatorManager>();

        // Act
        MocaleLocatorHelper.SetTranslatorManager(translatorManager.Object);

        // Assert
        Assert.Equal(translatorManager.Object, MocaleLocator.TranslatorManager);
    }
}

using Mocale.Abstractions;
using Mocale.Helper;

namespace Mocale.UnitTests.Helpers;

public class VersionPrefixHelperTests : FixtureBase
{
    #region Setup

    private readonly Mock<IConfigurationManager<IExternalProviderConfiguration>> configurationManager;

    public VersionPrefixHelperTests()
    {
        configurationManager = new Mock<IConfigurationManager<IExternalProviderConfiguration>>();
    }

    public override object CreateSystemUnderTest()
    {
        return new VersionPrefixHelper(configurationManager.Object);
    }

    #endregion Setup

    #region Tests

    [Theory]
    [InlineData("en-GB.json", null, "en-GB.json")]
    [InlineData("fr-FR.json", null, "fr-FR.json")]
    [InlineData("en-GB.json", "v1", "v1/en-GB.json")]
    [InlineData("fr-FR.json", "v1", "v1/fr-FR.json")]
    [InlineData("en-GB.json", "v2", "v2/en-GB.json")]
    [InlineData("fr-FR.json", "v2", "v2/fr-FR.json")]
    public void ApplyVersionPrefix_ShouldApplyPrefix_DependingOnConfiguration(string fileName, string prefix, string expectedFileName)
    {
        // Arrange
        var mockConfig = new Mock<IExternalProviderConfiguration>();

        mockConfig.SetupGet(m => m.VersionPrefix)
            .Returns(prefix);

        configurationManager.Setup(m => m.Configuration)
            .Returns(mockConfig.Object);

        var sut = GetSut<IVersionPrefixHelper>();

        // Act
        var result = sut.ApplyVersionPrefix(fileName);

        // Assert
        Assert.Equal(expectedFileName, result);
    }

    #endregion Tests
}

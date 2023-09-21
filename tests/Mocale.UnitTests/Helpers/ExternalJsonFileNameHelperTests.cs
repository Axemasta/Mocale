using System.Globalization;
using Mocale.Abstractions;
using Mocale.Helper;
using Mocale.Models;

namespace Mocale.UnitTests.Helpers;

public class ExternalJsonFileNameHelperTests : FixtureBase
{
    #region Setup

    private readonly Mock<IConfigurationManager<IExternalProviderConfiguration>> configurationManager;

    public ExternalJsonFileNameHelperTests()
    {
        configurationManager = new Mock<IConfigurationManager<IExternalProviderConfiguration>>();
    }

    public override object CreateSystemUnderTest()
    {
        return new ExternalJsonFileNameHelper(configurationManager.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void Constructor_WhenConfigIsWrongType_ShouldThrow()
    {
        // Arrange
        var resxFileConfig = new ResxResourceFileDetails()
        {
            ResourcePrefix = "test",
        };

        var config = new Mock<IExternalProviderConfiguration>();

        config.SetupGet(m => m.ResourceFileDetails)
            .Returns(resxFileConfig);

        configurationManager.SetupGet(m => m.Configuration)
            .Returns(config.Object);

        // Act
        var ex = Record.Exception(GetSut<IExternalProviderConfiguration>);

        // Assert
        Assert.IsType<NotSupportedException>(ex);
        Assert.Equal("Resource file details were not for json files", ex.Message);
    }

    [Theory]
    [InlineData("en-GB", null, "en-GB.json")]
    [InlineData("fr-FR", null, "fr-FR.json")]
    [InlineData("en-GB", "", "en-GB.json")]
    [InlineData("fr-FR", "", "fr-FR.json")]
    [InlineData("en-GB", "v1", "v1/en-GB.json")]
    [InlineData("fr-FR", "v1", "v1/fr-FR.json")]
    [InlineData("en-GB", "v2", "v2/en-GB.json")]
    [InlineData("fr-FR", "v2", "v2/fr-FR.json")]
    public void GetExpectedFileName_WhenCalled_ShouldCreateCorrectName(string cultureString, string? versionPrefix, string expectedFileName)
    {
        // Arrange
        var resxFileConfig = new JsonResourceFileDetails()
        {
            VersionPrefix = versionPrefix,
        };

        var config = new Mock<IExternalProviderConfiguration>();

        config.SetupGet(m => m.ResourceFileDetails)
            .Returns(resxFileConfig);

        configurationManager.SetupGet(m => m.Configuration)
            .Returns(config.Object);

        var culture = new CultureInfo(cultureString);

        var sut = GetSut<IExternalFileNameHelper>();

        // Act
        var fileName = sut.GetExpectedFileName(culture);

        // Arrange
        Assert.Equal(expectedFileName, fileName);
    }

    #endregion Tests
}

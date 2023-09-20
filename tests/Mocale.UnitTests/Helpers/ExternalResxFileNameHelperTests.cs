using System.Globalization;
using Mocale.Abstractions;
using Mocale.Helper;
using Mocale.Models;

namespace Mocale.UnitTests.Helpers;

public class ExternalResxFileNameHelperTests : FixtureBase
{
    #region Setup

    private readonly Mock<IConfigurationManager<IExternalProviderConfiguration>> configurationManager;

    public ExternalResxFileNameHelperTests()
    {
        configurationManager = new Mock<IConfigurationManager<IExternalProviderConfiguration>>();
    }

    public override object CreateSystemUnderTest()
    {
        return new ExternalResxFileNameHelper(configurationManager.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void Constructor_WhenConfigIsWrongType_ShouldThrow()
    {
        // Arrange
        var jsonFileConfig = new JsonResourceFileDetails();

        var config = new Mock<IExternalProviderConfiguration>();

        config.SetupGet(m => m.ResourceFileDetails)
            .Returns(jsonFileConfig);

        configurationManager.SetupGet(m => m.Configuration)
            .Returns(config.Object);

        // Act
        var ex = Record.Exception(GetSut<IExternalProviderConfiguration>);

        // Assert
        Assert.IsType<NotSupportedException>(ex);
        Assert.Equal("Resource file details were not for resx files", ex.Message);
    }

    [Theory]
    [InlineData("en-GB", null, "AppResources.en-GB.resx")]
    [InlineData("fr-FR", null, "AppResources.fr-FR.resx")]
    [InlineData("en-GB", "", "AppResources.en-GB.resx")]
    [InlineData("fr-FR", "", "AppResources.fr-FR.resx")]
    [InlineData("en-GB", "v1", "v1/AppResources.en-GB.resx")]
    [InlineData("fr-FR", "v1", "v1/AppResources.fr-FR.resx")]
    public void GetExpectedFileName_WhenCalled_ShouldCreateCorrectName(string cultureString, string? versionPrefix, string expectedFileName)
    {
        // Arrange
        var resxFileConfig = new ResxResourceFileDetails()
        {
            ResourcePrefix = "AppResources",
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

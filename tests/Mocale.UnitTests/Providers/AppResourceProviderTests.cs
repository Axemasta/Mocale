using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Mocale.Abstractions;
using Mocale.Exceptions;
using Mocale.Models;
using Mocale.Providers;

namespace Mocale.UnitTests.Providers;

public class AppResourceProviderTests : FixtureBase<IInternalLocalizationProvider>
{
    #region Setup

    private readonly Mock<IConfigurationManager<IAppResourcesConfig>> appResourcesConfigManager = new();
    private readonly Mock<IConfigurationManager<IMocaleConfiguration>> mocaleConfigurationManager = new();
    private readonly Mock<ILogger<AppResourceProvider>> logger = new();

    public override IInternalLocalizationProvider CreateSystemUnderTest()
    {
        return new AppResourceProvider(
            appResourcesConfigManager.Object,
            mocaleConfigurationManager.Object,
            logger.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void Constructor_WhenAppResourcesTypeIsNull_ShouldThrow()
    {
        // Arrange
        appResourcesConfigManager.Setup(m => m.Configuration)
            .Returns(new AppResourcesConfig()
            {
                AppResourcesType = null,
            });

        mocaleConfigurationManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration());

        // Act
        var ex = Assert.Throws<InitializationException>(() => new AppResourceProvider(
            appResourcesConfigManager.Object,
            mocaleConfigurationManager.Object,
            logger.Object));

        // Assert
        Assert.Equal("App Resource Type has not been set, this should be configured during startup", ex.Message);
    }

    [Fact]
    public void Constructor_WhenAppResourcesTypeIsNotResx_ShouldReturnNull()
    {
        // Arrange
        appResourcesConfigManager.Setup(m => m.Configuration)
            .Returns(new AppResourcesConfig()
            {
                AppResourcesType = typeof(string),
            });

        mocaleConfigurationManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration());

        // Act
        var values = Sut.GetValuesForCulture(new CultureInfo("en-GB"));

        // Assert
        Assert.Null(values);
        logger.VerifyLog(log => log.LogWarning("Unable to load default resource set"), Times.Once);
    }

    [Fact]
    public void GetValuesForCulture_WhenCultureIsDefault_ShouldLoadTranslations()
    {
        // Arrange
        appResourcesConfigManager.Setup(m => m.Configuration)
            .Returns(new AppResourcesConfig()
            {
                AppResourcesType = typeof(Resources.Resx.TestResources),
            });

        mocaleConfigurationManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                DefaultCulture = new CultureInfo("en-GB"),
            });

        // Act
        var values = Sut.GetValuesForCulture(new CultureInfo("en-GB"));

        // Assert
        Assert.NotNull(values);
        Assert.Equal(2, values.Count);
        Assert.Equal("Value One", values["KeyOne"]);
        Assert.Equal("Value Two", values["KeyTwo"]);
    }

    [Fact]
    public void GetValuesForCulture_WhenCultureIsPresentButNotDefault_ShouldLoadTranslations()
    {
        // Arrange
        appResourcesConfigManager.Setup(m => m.Configuration)
            .Returns(new AppResourcesConfig()
            {
                AppResourcesType = typeof(Resources.Resx.TestResources),
            });

        mocaleConfigurationManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                DefaultCulture = new CultureInfo("en-GB"),
            });

        // Act
        var values = Sut.GetValuesForCulture(new CultureInfo("fr-FR"));

        // Assert
        Assert.NotNull(values);
        Assert.Equal(2, values.Count);
        Assert.Equal("Clé un", values["KeyOne"]);
        Assert.Equal("Clé deux", values["KeyTwo"]);
    }

    [Fact]
    public void GetValuesForCulture_WhenCultureIsNotPresent_ShouldReturnNull()
    {
        // Arrange
        appResourcesConfigManager.Setup(m => m.Configuration)
            .Returns(new AppResourcesConfig()
            {
                AppResourcesType = typeof(Resources.Resx.TestResources),
            });

        mocaleConfigurationManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                DefaultCulture = new CultureInfo("en-GB"),
            });

        // Act
        var values = Sut.GetValuesForCulture(new CultureInfo("it-IT"));

        // Assert
        Assert.Null(values);

        logger.VerifyLog(log => log.LogWarning("No resources found for culture {CultureName}", "it-IT"),
            Times.Once);
    }

    #endregion Tests
}

using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Providers;
namespace Mocale.UnitTests.Providers;

public class EmbeddedResourceProviderTests : FixtureBase<IInternalLocalizationProvider>
{
    #region Setup

    private readonly Mock<IConfigurationManager<IEmbeddedResourcesConfig>> configurationManager;
    private readonly Mock<IEmbeddedResourcesConfig> embeddedResourcesConfig;
    private readonly Mock<ILogger<EmbeddedResourceProvider>> logger;

    public EmbeddedResourceProviderTests()
    {
        configurationManager = new Mock<IConfigurationManager<IEmbeddedResourcesConfig>>();
        embeddedResourcesConfig = new Mock<IEmbeddedResourcesConfig>();
        logger = new Mock<ILogger<EmbeddedResourceProvider>>();

        configurationManager.SetupGet(m => m.Configuration)
            .Returns(embeddedResourcesConfig.Object);
    }

    public override IInternalLocalizationProvider CreateSystemUnderTest()
    {
        return new EmbeddedResourceProvider(
            configurationManager.Object,
            logger.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void GetValuesForCulture_WhenResourceAssemblyIsNull_ShouldReturnNull()
    {
        // Arrange
        embeddedResourcesConfig.SetupGet(m => m.ResourcesAssembly)
            .Returns(() => null);

        var culture = new CultureInfo("en-GB");

        // Act
        var values = Sut.GetValuesForCulture(culture);

        // Assert
        Assert.Null(values);

        logger.VerifyLog(log => log.LogWarning("Configured resource assembly was null"),
            Times.Once());
    }

    [Fact]
    public void GetValuesForCulture_WhenResourceFolderNotFound_ShouldReturnNull()
    {
        // Arrange
        embeddedResourcesConfig.SetupGet(m => m.ResourcesAssembly)
            .Returns(typeof(EmbeddedResourceProviderTests).Assembly);

        embeddedResourcesConfig.SetupGet(m => m.UseResourceFolder)
            .Returns(true);

        embeddedResourcesConfig.SetupGet(m => m.ResourcesPath)
            .Returns("Locales");

        var culture = new CultureInfo("en-GB");

        // Act
        var values = Sut.GetValuesForCulture(culture);

        // Assert
        Assert.Null(values);

        logger.VerifyLog(log => log.LogWarning("No assembly resources found with prefix: {FolderPrefix}", "Mocale.UnitTests.Resources.Locales"),
            Times.Once());
    }

    [Fact]
    public void GetValuesForCulture_WhenResourceFolderFoundButNoMatchingFiles_ShouldReturnNull()
    {
        // Arrange
        embeddedResourcesConfig.SetupGet(m => m.ResourcesAssembly)
            .Returns(typeof(EmbeddedResourceProviderTests).Assembly);

        embeddedResourcesConfig.SetupGet(m => m.UseResourceFolder)
            .Returns(false);

        embeddedResourcesConfig.SetupGet(m => m.ResourcesPath)
            .Returns("Resources.Misc");

        var culture = new CultureInfo("en-GB");

        // Act
        var values = Sut.GetValuesForCulture(culture);

        // Assert
        Assert.Null(values);

        logger.VerifyLog(log => log.LogWarning("Unable to find resource for selected culture: {CultureName}", "en-GB"),
            Times.Once());
    }

    [Fact]
    public void GetValuesForCulture_WhenResourceFolderContainsInvalidCultureFiles_ShouldReturnNull()
    {
        // Arrange
        embeddedResourcesConfig.SetupGet(m => m.ResourcesAssembly)
            .Returns(typeof(EmbeddedResourceProviderTests).Assembly);

        embeddedResourcesConfig.SetupGet(m => m.UseResourceFolder)
            .Returns(true);

        embeddedResourcesConfig.SetupGet(m => m.ResourcesPath)
            .Returns("Invalid");

        var culture = new CultureInfo("en-GB");

        // Act
        var values = Sut.GetValuesForCulture(culture);

        // Assert
        Assert.Null(values);

        logger.VerifyLog(
            log => log.LogError(
                It.IsAny<Exception>(),
                "An exception occurred loading & parsing assembly resource {FilePath}",
                It.IsAny<string>()),
            Times.Once());
    }

    [Fact]
    public void GetValuesForCulture_WhenResourceFolderContainsValidCultureFiles_ShouldReturnLocalizations()
    {
        // Arrange
        embeddedResourcesConfig.SetupGet(m => m.ResourcesAssembly)
            .Returns(typeof(EmbeddedResourceProviderTests).Assembly);

        embeddedResourcesConfig.SetupGet(m => m.UseResourceFolder)
            .Returns(true);

        var culture = new CultureInfo("en-GB");

        // Act
        var values = Sut.GetValuesForCulture(culture);

        // Assert
        Assert.NotNull(values);
        Assert.NotEmpty(values);

        var expectedLocalizations = new Dictionary<string, string>()
        {
            {
                "CurrentLocaleName", "English"
            },
            {
                "LocalizationCurrentProviderIs", "The current localization provider is:"
            },
            {
                "LocalizationProviderName", "Json"
            },
            {
                "MocaleDescription", "Localization framework for .NET Maui"
            },
            {
                "MocaleTitle", "Mocale"
            },
            {
                "ExternalPrefixExplanation", "Strings prefixed with GR_ indicate they have been pulled from the external provider (GitHub.Raw), when the local cache expires if these values change, so will the text displayed!"
            },
        };

        values.Should()
            .BeEquivalentTo(expectedLocalizations);
    }

    [Fact]
    public void ParseFile_WhenFileStreamIsNull_ShouldLogAndReturnNull()
    {
        // Arrange
        var sut = (EmbeddedResourceProvider)Sut;


        // Act
        var translations = sut.ParseFile("ThisIsNotAValidPath!", typeof(EmbeddedResourceProviderTests).Assembly);

        // Assert
        Assert.Null(translations);
        logger.VerifyLog(log => log.LogWarning(
                "File stream was null for assembly resource: {FilePath}",
                "ThisIsNotAValidPath!"),
            Times.Once());
    }

    #endregion Tests
}

using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Providers.GitHub.Raw;
using Mocale.Providers.GitHub.Raw.Abstractions;
using Mocale.Providers.GitHub.Raw.Models;
using Moq.Contrib.HttpClient;
namespace Mocale.UnitTests.Providers;

public class GitHubRawProviderTests : FixtureBase<IExternalLocalizationProvider>
{
    #region Setup

    private readonly Mock<IConfigurationManager<IGithubRawConfig>> configurationManager;
    private readonly Mock<HttpMessageHandler> httpMessageHandler;
    private readonly Mock<ILogger<GitHubRawProvider>> logger;

    #region Constructors

    public GitHubRawProviderTests()
    {
        configurationManager = new Mock<IConfigurationManager<IGithubRawConfig>>();
        httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        logger = new Mock<ILogger<GitHubRawProvider>>();
    }

    #endregion Constructors

    public override IExternalLocalizationProvider CreateSystemUnderTest()
    {
        return new GitHubRawProvider(
            configurationManager.Object,
            new HttpClient(httpMessageHandler.Object),
            logger.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public async Task GetValuesForCultureAsync_WhenResourceExistAndIsValidLocalization_ShouldDeserializeAndReturnSuccess()
    {
        // Arrange
        var newCulture = new CultureInfo("en-GB");

        configurationManager.SetupGet(m => m.Configuration)
            .Returns(new GithubRawConfig()
            {
                Username = "Axemasta",
                Repository = "Mocale",
                Branch = "main",
                LocaleDirectory = "samples/Locales",
            });

        var rawJson = """
            {
              "CurrentLocaleName": "English",
              "LocalizationCurrentProviderIs": "GR_The current localization provider is:",
              "LocalizationProviderName": "GR_Json",
              "MocaleDescription": "GR_Localization framework for .NET Maui",
              "MocaleTitle": "GR_Mocale"
            }
            """;

        httpMessageHandler.SetupRequest(HttpMethod.Get, "https://raw.githubusercontent.com/Axemasta/Mocale/main/samples/Locales/en-GB.json")
            .ReturnsResponse(rawJson);

        // Act
        var result = await Sut.GetValuesForCultureAsync(newCulture);

        // Assert
        Assert.True(result.Success);

        var expectedLocalizations = new Dictionary<string, string>()
        {
            { "CurrentLocaleName", "English" },
            { "LocalizationCurrentProviderIs", "GR_The current localization provider is:" },
            { "LocalizationProviderName", "GR_Json" },
            { "MocaleDescription", "GR_Localization framework for .NET Maui" },
            { "MocaleTitle", "GR_Mocale" },
        };

        result.Localizations.Should().BeEquivalentTo(expectedLocalizations);
    }

    #endregion Tests
}

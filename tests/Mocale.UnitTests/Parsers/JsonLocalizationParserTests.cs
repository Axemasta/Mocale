using System.Text;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Parsers;

namespace Mocale.UnitTests.Parsers;

public class JsonLocalizationParserTests : FixtureBase
{
    #region Setup

    private readonly Mock<ILogger<JsonLocalizationParser>> logger;

    public JsonLocalizationParserTests()
    {
        logger = new Mock<ILogger<JsonLocalizationParser>>();
    }

    public override object CreateSystemUnderTest()
    {
        return new JsonLocalizationParser(logger.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void ParseLocalizationStream_WhenStreamIsNotJson_ShouldReturnNull()
    {
        // Arrange
        var sut = GetSut<ILocalizationParser>();

        var notJson = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas diam sapien, posuere vel arcu sit amet, tempus tincidunt eros. Duis fringilla ligula eu massa cursus facilisis. Donec congue eget eros at porttitor. Maecenas sed nisl augue. Nam vel ex neque. Nam posuere convallis mauris, eu fringilla neque tempor sit amet. Nullam sollicitudin dolor eu justo aliquet cursus. Curabitur et risus vel elit viverra iaculis congue vel ex. Sed mollis pretium interdum.";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(notJson));

        // Act
        var localizations = sut.ParseLocalizationStream(stream);

        // Assert
        Assert.Null(localizations);

        logger.VerifyLog(
            log => log.LogError(
                It.IsAny<Exception>(),
                It.IsAny<string>()),
            Times.Once());
    }

    [Fact]
    public void ParseLocalizationStream_WhenStreamIsJsonButNotLocalizations_ShouldReturnNull()
    {
        // Arrange
        var sut = GetSut<ILocalizationParser>();

        var json = /*lang=json,strict*/ """
            {
            	"name": "Luke Skywalker",
            	"height": "172",
            	"mass": "77",
            	"hair_color": "blond",
            	"skin_color": "fair",
            	"eye_color": "blue",
            	"birth_year": "19BBY",
            	"gender": "male",
            	"homeworld": "https://swapi.dev/api/planets/1/",
            	"films": [
            		"https://swapi.dev/api/films/2/",
            		"https://swapi.dev/api/films/6/",
            		"https://swapi.dev/api/films/3/",
            		"https://swapi.dev/api/films/1/",
            		"https://swapi.dev/api/films/7/"
            	],
            	"species": [
            		"https://swapi.dev/api/species/1/"
            	],
            	"vehicles": [
            		"https://swapi.dev/api/vehicles/14/",
            		"https://swapi.dev/api/vehicles/30/"
            	],
            	"starships": [
            		"https://swapi.dev/api/starships/12/",
            		"https://swapi.dev/api/starships/22/"
            	],
            	"created": "2014-12-09T13:50:51.644000Z",
            	"edited": "2014-12-20T21:17:56.891000Z",
            	"url": "https://swapi.dev/api/people/1/"
            }
            """;

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var localizations = sut.ParseLocalizationStream(stream);

        // Assert
        Assert.Null(localizations);

        logger.VerifyLog(
            log => log.LogError(
                It.IsAny<Exception>(),
                It.IsAny<string>()),
            Times.Once());
    }

    [Fact]
    public void ParseLocalizationStream_WhenStreamIsLocalizationJson_ShouldReturnExpectedResponse()
    {
        // Arrange
        var sut = GetSut<ILocalizationParser>();

        var json = /*lang=json,strict*/ """
            {
            	"CurrentLocaleName": "English",
                "LocalizationCurrentProviderIs": "The current localization provider is:",
                "LocalizationProviderName": "Json",
                "MocaleDescription": "Localization framework for .NET Maui",
                "MocaleTitle": "Mocale"
            }
            """;

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var localizations = sut.ParseLocalizationStream(stream);

        // Assert

        var expectedLocalizations = new Dictionary<string, string>()
        {
            { "CurrentLocaleName", "English" },
            { "LocalizationCurrentProviderIs", "The current localization provider is:" },
            { "LocalizationProviderName", "Json" },
            { "MocaleDescription", "Localization framework for .NET Maui" },
            { "MocaleTitle", "Mocale" },
        };

        localizations.Should().BeEquivalentTo(expectedLocalizations);
    }

    #endregion Tests
}

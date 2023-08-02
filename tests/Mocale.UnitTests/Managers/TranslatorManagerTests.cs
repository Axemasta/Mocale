using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;
using Moq;

namespace Mocale.UnitTests.Managers;

public class TranslatorManagerTests : FixtureBase<TranslatorManager>
{
    #region Setup

    private readonly Mock<ILogger<TranslatorManager>> logger;
    private readonly Mock<IConfigurationManager<IMocaleConfiguration>> configManager;

    public TranslatorManagerTests()
    {
        logger = new Mock<ILogger<TranslatorManager>>();
        configManager = new Mock<IConfigurationManager<IMocaleConfiguration>>();
    }

    public override TranslatorManager CreateSystemUnderTest()
    {
        return new TranslatorManager(logger.Object, configManager.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void Translate_WhenExternalLocalizationsContainsKey_ShouldReturnLocalization()
    {
        // Arrange
        var translations = new Dictionary<string,string>()
        {
            { "KeyOne", "I am the first key" },
        };

        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                {
                    "KeyOne", "I am the first key"
                },
            }
        };

        Sut.UpdateTranslations(localization, TranslationSource.External);

        // Act
        var translation = Sut.Translate("KeyOne");

        // Assert
        Assert.Equal("I am the first key", translation);
    }

    #endregion Tests
}


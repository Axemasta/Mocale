using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;
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

        logger.VerifyLog(log => log.LogDebug("Key: {Key} was found in backup localizations", "KeyOne"), Times.Never());
    }

    [Fact]
    public void Translate_WhenInternalLocalizationsContainsKey_ShouldReturnLocalization()
    {
        // Arrange
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

        Sut.UpdateTranslations(localization, TranslationSource.Internal);

        // Act
        var translation = Sut.Translate("KeyOne");

        // Assert
        Assert.Equal("I am the first key", translation);

        logger.VerifyLog(log => log.LogDebug("Key: {Key} was found in backup localizations", "KeyOne"), Times.Once());
    }

    [Fact]
    public void Translate_WhenNoLocalizationsForKeyAndShowMissingKeysIsDisabled_ShouldReturnEmpty()
    {
        // Arrange
        configManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                ShowMissingKeys = false,
            });

        // Act
        var translation = Sut.Translate("KeyOne");

        // Assert
        Assert.Equal(string.Empty, translation);

        logger.VerifyLog(log => log.LogWarning("Resource key not found '{ResourceKey}'", "KeyOne"), Times.Once());
    }

    [Theory]
    [InlineData("$", "$KeyOne$")]
    [InlineData("?", "?KeyOne?")]
    [InlineData("_", "_KeyOne_")]
    [InlineData("__", "__KeyOne__")]
    [InlineData("_=", "_=KeyOne=_")]
    public void Translate_WhenNoLocalizationsForKeyAndShowMissingKeysIsEnabled_ShouldFormattedMissingKey(string missingSymbol, string expectedTranslation)
    {
        // Arrange
        configManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                ShowMissingKeys = true,
                NotFoundSymbol = missingSymbol,
            });

        // Act
        var translation = Sut.Translate("KeyOne");

        // Assert
        Assert.Equal(expectedTranslation, translation);

        logger.VerifyLog(log => log.LogWarning("Resource key not found '{ResourceKey}'", "KeyOne"), Times.Once());
    }

    [Fact]
    public void Translate_WhenCultureChanges_ShouldReturnNewLocalization()
    {
        // Arrange
        var englishLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                {
                    "KeyOne", "I am the first key"
                },
            }
        };

        var frenchLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                {
                    "KeyOne", "je suis la première clé"
                },
            }
        };

        Sut.UpdateTranslations(englishLocalization, TranslationSource.External);

        // Act
        Sut.UpdateTranslations(frenchLocalization, TranslationSource.External);

        var translation = Sut.Translate("KeyOne");

        // Assert
        Assert.Equal("je suis la première clé", translation);
    }

    [Fact]
    public void Translate_WhenCultureChangesAndNewCultureDoesntContainKey_ShouldReturnMissing()
    {
        // Arrange
        var englishLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                {
                    "KeyOne", "I am the first key"
                },
            }
        };

        var frenchLocalization = new Localization()
        {
            CultureInfo = new CultureInfo("fr-FR"),
            Translations = new Dictionary<string, string>()
            {
                {
                    "KeyTwo", "je suis la deuxième clé"
                },
            }
        };

        configManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                ShowMissingKeys = true,
                NotFoundSymbol = "_",
            });

        Sut.UpdateTranslations(englishLocalization, TranslationSource.External);

        // Act
        Sut.UpdateTranslations(frenchLocalization, TranslationSource.External);

        var translation = Sut.Translate("KeyOne");

        // Assert
        Assert.Equal("_KeyOne_", translation);
    }

    #endregion Tests
}

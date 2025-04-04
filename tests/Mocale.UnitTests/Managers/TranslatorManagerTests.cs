using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale.UnitTests.Managers;

public class TranslatorManagerTests : FixtureBase
{
    #region Setup

    private readonly Mock<ILogger<TranslatorManager>> logger = new();
    private readonly Mock<IConfigurationManager<IMocaleConfiguration>> configManager = new();

    public override object CreateSystemUnderTest()
    {
        return new TranslatorManager(logger.Object, configManager.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void UpdateTranslations_WhenCurrentCultureIsNotSet_ShouldSetCurrentCultureAndClearPreviousValues()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();
        sut.PreferredLocalizations.Add("KeyOne", "Value One");
        sut.BackupLocalizations.Add("BackupKeyOne", "Backup Value One");

        Assert.Null(sut.CurrentCulture);

        var localization = new Localization(new CultureInfo("en-GB"));

        // Act
        sut.UpdateTranslations(localization, TranslationSource.Internal);

        // Assert
        Assert.Equal(new CultureInfo("en-GB"), sut.CurrentCulture);
        Assert.Empty(sut.PreferredLocalizations);
        Assert.Empty(sut.BackupLocalizations);
    }

    [Fact]
    public void UpdateTranslations_WhenTranslationSourceIsInternal_ShouldUpdateBackupLocalizations()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();

        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World" },
            }
        };

        // Act
        sut.UpdateTranslations(localization, TranslationSource.Internal);

        // Assert
        Assert.Equal(new CultureInfo("en-GB"), sut.CurrentCulture);
        Assert.Empty(sut.PreferredLocalizations);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "HelloWorld", "Hello World" },
        }, sut.BackupLocalizations);
    }

    [Fact]
    public void UpdateTranslations_WhenTranslationSourceIsExternal_ShouldUpdatePreferredLocalizations()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();

        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World" },
            }
        };

        // Act
        sut.UpdateTranslations(localization, TranslationSource.External);

        // Assert
        Assert.Equal(new CultureInfo("en-GB"), sut.CurrentCulture);
        Assert.Empty(sut.BackupLocalizations);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "HelloWorld", "Hello World" },
        }, sut.PreferredLocalizations);
    }

    [Fact]
    public void UpdateTranslations_WhenTranslationSourceIsWarmCache_ShouldUpdatePreferredLocalizations()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();

        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World" },
            }
        };

        // Act
        sut.UpdateTranslations(localization, TranslationSource.WarmCache);

        // Assert
        Assert.Equal(new CultureInfo("en-GB"), sut.CurrentCulture);
        Assert.Empty(sut.BackupLocalizations);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "HelloWorld", "Hello World" },
        }, sut.PreferredLocalizations);
    }

    [Fact]
    public void UpdateTranslations_WhenTranslationSourceIsColdCache_ShouldUpdatePreferredLocalizations()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();

        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World" },
            }
        };

        // Act
        sut.UpdateTranslations(localization, TranslationSource.ColdCache);

        // Assert
        Assert.Equal(new CultureInfo("en-GB"), sut.CurrentCulture);
        Assert.Empty(sut.BackupLocalizations);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "HelloWorld", "Hello World" },
        }, sut.PreferredLocalizations);
    }

    [Fact]
    public void UpdateTranslations_WhenNotify_ShouldRaisePropertyChanged()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();

        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World" },
            }
        };

        var invocations = 0;
        sut.PropertyChanged += (_, _) =>
        {
            invocations++;
        };

        // Act
        sut.UpdateTranslations(localization, TranslationSource.External, true);

        // Assert
        Assert.Equal(new CultureInfo("en-GB"), sut.CurrentCulture);
        Assert.Equal(1, invocations);
    }

    [Fact]
    public void UpdateTranslations_WhenNotNotify_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();

        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "HelloWorld", "Hello World" },
            }
        };

        var invocations = 0;
        sut.PropertyChanged += (_, _) =>
        {
            invocations++;
        };

        // Act
        sut.UpdateTranslations(localization, TranslationSource.External, false);

        // Assert
        Assert.Equal(new CultureInfo("en-GB"), sut.CurrentCulture);
        Assert.Equal(0, invocations);
    }

    [Fact]
    public void Translate_WhenExternalLocalizationsContainsKey_ShouldReturnLocalization()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string> { { "KeyOne", "I am the first key" } }
        };

        var sut = GetSut<TranslatorManager>();

        sut.UpdateTranslations(localization, TranslationSource.External);

        // Act
        var translation = sut.Translate("KeyOne");

        // Assert
        Assert.Equal("I am the first key", translation);

        logger.VerifyLog(log => log.LogDebug("Key: {Key} was found in backup localizations", "KeyOne"), Times.Never());
    }

    [Fact]
    public void Translate_WhenInternalLocalizationsContainsKey_ShouldReturnLocalization()
    {
        // Arrange
        var localization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string> { { "KeyOne", "I am the first key" } }
        };

        var sut = GetSut<TranslatorManager>();

        sut.UpdateTranslations(localization, TranslationSource.Internal);

        // Act
        var translation = sut.Translate("KeyOne");

        // Assert
        Assert.Equal("I am the first key", translation);

        logger.VerifyLog(log => log.LogDebug("Key: {Key} was found in backup localizations", "KeyOne"), Times.Once());
    }

    [Fact]
    public void Translate_WhenNoLocalizationsForKeyAndShowMissingKeysIsDisabled_ShouldReturnEmpty()
    {
        // Arrange
        configManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration { ShowMissingKeys = false });

        var sut = GetSut<TranslatorManager>();

        // Act
        var translation = sut.Translate("KeyOne");

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
    public void Translate_WhenNoLocalizationsForKeyAndShowMissingKeysIsEnabled_ShouldFormattedMissingKey(
        string missingSymbol, string expectedTranslation)
    {
        // Arrange
        configManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration { ShowMissingKeys = true, NotFoundSymbol = missingSymbol });

        var sut = GetSut<TranslatorManager>();

        // Act
        var translation = sut.Translate("KeyOne");

        // Assert
        Assert.Equal(expectedTranslation, translation);

        logger.VerifyLog(log => log.LogWarning("Resource key not found '{ResourceKey}'", "KeyOne"), Times.Once());
    }

    [Fact]
    public void Translate_WhenCultureChanges_ShouldReturnNewLocalization()
    {
        // Arrange
        var englishLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string> { { "KeyOne", "I am the first key" } }
        };

        var frenchLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string> { { "KeyOne", "je suis la première clé" } }
        };

        var sut = GetSut<TranslatorManager>();

        sut.UpdateTranslations(englishLocalization, TranslationSource.External);

        // Act
        sut.UpdateTranslations(frenchLocalization, TranslationSource.External);

        var translation = sut.Translate("KeyOne");

        // Assert
        Assert.Equal("je suis la première clé", translation);
    }

    [Fact]
    public void Translate_WhenCultureChangesAndNewCultureDoesntContainKey_ShouldReturnMissing()
    {
        // Arrange
        var englishLocalization = new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string> { { "KeyOne", "I am the first key" } }
        };

        var frenchLocalization = new Localization(new CultureInfo("fr-FR"))
        {
            Translations = new Dictionary<string, string> { { "KeyTwo", "je suis la deuxième clé" } }
        };

        configManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration { ShowMissingKeys = true, NotFoundSymbol = "_" });

        var sut = GetSut<TranslatorManager>();

        sut.UpdateTranslations(englishLocalization, TranslationSource.External);

        // Act
        sut.UpdateTranslations(frenchLocalization, TranslationSource.External);

        var translation = sut.Translate("KeyOne");

        // Assert
        Assert.Equal("_KeyOne_", translation);
    }

    [Fact]
    public void TranslateWithParameters_WhenTranslatedKeyIsEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        configManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                ShowMissingKeys = false,
            });

        var sut = GetSut<TranslatorManager>();

        // Act
        var translation = sut.Translate("MissingKey", [123, 456]);

        // Assert
        Assert.Equal(string.Empty, translation);
    }

    [Fact]
    public void TranslateWithParameters_WhenTranslatedKeyIsMissingKey_ShouldReturnMissingKey()
    {
        // Arrange
        configManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                ShowMissingKeys = true,
                NotFoundSymbol = "$"
            });

        var sut = GetSut<TranslatorManager>();

        // Act
        var translation = sut.Translate("MissingKey", [123, 456]);

        // Assert
        Assert.Equal("$MissingKey$", translation);
    }

    [Fact]
    public void TranslateWithParameters_WhenNoFormatString_ShouldReturnTranslation()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();
        sut.PreferredLocalizations.Add("FormatKey", "Hello stranger, welcome!");

        // Act
        var translation = sut.Translate("FormatKey", ["Stranger"]);

        // Assert
        Assert.Equal("Hello stranger, welcome!", translation);
    }

    [Fact]
    public void TranslateWithParameters_WhenFormatStringInvalid_ShouldCatchExceptionAndReturnEmptyString()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();
        sut.PreferredLocalizations.Add("TooManyFormats", "Hello {0}, welcome to {1} where we are {2} tonight!!");

        // Act
        var translation = sut.Translate("TooManyFormats", ["Stranger"]);

        // Assert
        Assert.Equal("Hello {0}, welcome to {1} where we are {2} tonight!!", translation);

        logger.VerifyLog(log => log.LogError(
            It.IsAny<Exception>(),
            "An exception occurred formating translation for key {Key}: '{Translation}' with parameters: {Parameters}",
            "TooManyFormats",
            "Hello {0}, welcome to {1} where we are {2} tonight!!",
            new object[] { "Stranger" }), Times.Once);
    }

    [Fact]
    public void TranslateWithParameters_FormatStringValid_ShouldFormatCorrectly()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();
        sut.PreferredLocalizations.Add("TooManyFormats", "Hello {0}, welcome to {1} where we are {2} tonight!!");

        // Act
        var translation = sut.Translate("TooManyFormats", ["Stranger", "the shop", "buying rare things"]);

        // Assert
        Assert.Equal("Hello Stranger, welcome to the shop where we are buying rare things tonight!!", translation);
    }

    [Fact]
    public void Translate_WhenUsingExtension_ShouldTranslate()
    {
        // Arrange
        var sut = GetSut<TranslatorManager>();

        sut.PreferredLocalizations.Add("KeyOne", "Value One");

        // Act
        var translation = sut["KeyOne"];

        // Assert
        Assert.Equal("Value One", translation);
    }

    #endregion Tests
}

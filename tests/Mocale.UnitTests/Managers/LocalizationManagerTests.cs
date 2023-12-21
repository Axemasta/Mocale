using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale.UnitTests.Managers;

public class LocalizationManagerTests : FixtureBase<ILocalizationManager>
{
    #region Setup

    private readonly Mock<ICurrentCultureManager> currentCultureManager;
    private readonly Mock<IConfigurationManager<IMocaleConfiguration>> configurationManager;
    private readonly Mock<ILogger<LocalizationManager>> logger;
    private readonly Mock<IMocaleConfiguration> mocaleConfiguration;
    private readonly Mock<ITranslationResolver> translationResolver;
    private readonly Mock<ITranslationUpdater> translationUpdater;

    public LocalizationManagerTests()
    {
        currentCultureManager = new Mock<ICurrentCultureManager>();
        configurationManager = new Mock<IConfigurationManager<IMocaleConfiguration>>();
        logger = new Mock<ILogger<LocalizationManager>>();
        mocaleConfiguration = new Mock<IMocaleConfiguration>();
        translationResolver = new Mock<ITranslationResolver>();
        translationUpdater = new Mock<ITranslationUpdater>();

        mocaleConfiguration = new Mock<IMocaleConfiguration>();

        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(true);

        configurationManager.SetupGet(m => m.Configuration)
            .Returns(mocaleConfiguration.Object);
    }

    public override ILocalizationManager CreateSystemUnderTest()
    {
        return new LocalizationManager(
            currentCultureManager.Object,
            configurationManager.Object,
            logger.Object,
            translationResolver.Object,
            translationUpdater.Object
            );
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void CurrentCulture_ShouldGetSetInConstructor()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        // Act

        // Assert
        Assert.Equal(activeCulture, Sut.CurrentCulture);
    }

    [Fact]
    public async Task Initialize_WhenExceptionIsThrown_ShouldLogAndReturnFalse()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var exception = new Exception("Loading culture exploded");

        translationResolver.Setup(m => m.LoadLocalTranslations(It.IsAny<CultureInfo>()))
            .Throws(exception);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.False(initialized);

        logger.VerifyLog(
            log => log.LogError(exception, "An exception occurred initializing LocalizationManager"),
            Times.Once());
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsArentLoaded_ShouldReturnFalse()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(new TranslationLoadResult()
            {
                Loaded = false,
                Localization = Localization.Invariant,
            });

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.False(initialized);

        logger.VerifyLog(
            log => log.LogWarning("Unable to load translations for culture: {CultureName}", activeCulture.Name),
            Times.Once());
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsAreHot_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var loadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                    {
                        { "KeyOne", "Ciao mondo!" },
                    }
            },
            Source = TranslationSource.WarmCache,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(loadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.WarmCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.WarmCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Never());
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsAreCold_ShouldUpdateTranslationsAndTriggerUpdateAndReturnTrue()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var loadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                    {
                        { "KeyOne", "Ciao mondo!" },
                    }
            },
            Source = TranslationSource.ColdCache,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(loadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.ColdCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.ColdCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Once());
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsAreInternal_ShouldUpdateTranslationsAndTriggerUpdateAndReturnTrue()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var loadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                    {
                        { "KeyOne", "Ciao mondo!" },
                    }
            },
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(loadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.Internal),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.Internal),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Once());
    }

    [Fact]
    public async Task Initialize_WhenTranslationsCanBeUpdatedAndExternalFailsToLoad_ShouldLogFailedUpdatedAndReturnTrue()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var localLoadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                    {
                        { "KeyOne", "Ciao mondo!" },
                    }
            },
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(localLoadResult);

        var externalLoadResult = new TranslationLoadResult()
        {
            Loaded = false,
            Localization = Localization.Invariant,
            Source = TranslationSource.External,
        };

        translationResolver.Setup(m => m.LoadTranslations(activeCulture))
            .ReturnsAsync(externalLoadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Small delay to reduce test flake on CI due to fire & forget call
        // maybe this is tech debt?
        await Task.Delay(50);

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal),
            Times.Once());

        translationUpdater.Verify(
            m => m.UpdateTranslations(It.IsAny<Localization>(), TranslationSource.External),
            Times.Never());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.Internal),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Once());

        logger.VerifyLog(
            log => log.LogWarning("Unable to load external translations for culture: {CultureInfo}", activeCulture),
            Times.Once());
    }

    [Fact]
    public async Task Initialize_WhenTranslationsCanBeUpdatedAndExternalLoads_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var localLoadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                    {
                        { "KeyOne", "Ciao mondo!" },
                    }
            },
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(localLoadResult);

        var externalLoadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "External Ciao mondo!" },
                }
            },
            Source = TranslationSource.External,
        };

        translationResolver.Setup(m => m.LoadTranslations(activeCulture))
            .ReturnsAsync(externalLoadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Small delay to reduce test flake on CI due to fire & forget call
        // maybe this is tech debt?
        await Task.Delay(50);

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal),
            Times.Once());

        translationUpdater.Verify(
            m => m.UpdateTranslations(externalLoadResult.Localization, TranslationSource.External),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.Internal),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Once());

        logger.VerifyLog(
            log => log.LogWarning("Unable to load external translations for culture: {CultureInfo}", activeCulture),
            Times.Never());
    }

    [Fact]
    public async Task Initialize_WhenExternalProviderNotEnabledAndLocalProviderFailsToLoad_ShouldLogAndReturnFalse()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(false);

        var activeCulture = new CultureInfo("fr-FR");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(new TranslationLoadResult()
            {
                Loaded = false,
                Localization = Localization.Invariant,
            });

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.False(initialized);

        logger.VerifyLog(
            log => log.LogWarning("Unable to load translations for culture: {CultureName}", activeCulture.Name),
            Times.Once());
    }

    [Fact]
    public async Task Initialize_WhenExternalProviderNotEnabledAndLocalProviderLoadsFromInternal_ShouldUpdateAndReturnTrueWithoutUpdatingExternal()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(false);

        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var loadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "Ciao mondo!" },
                }
            },
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(loadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.Internal),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.Internal),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Never());
    }

    [Fact]
    public async Task Initialize_WhenExternalProviderNotEnabledAndLocalProviderLoadsFromColdCache_ShouldUpdateAndReturnTrueWithoutUpdatingExternal()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(false);

        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var loadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "Ciao mondo!" },
                }
            },
            Source = TranslationSource.ColdCache,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(loadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.ColdCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.ColdCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Never());
    }

    [Fact]
    public async Task Initialize_WhenExternalProviderNotEnabledAndLocalProviderLoadsFromHotCache_ShouldUpdateAndReturnTrueWithoutUpdatingExternal()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(false);

        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var loadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = activeCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "Ciao mondo!" },
                }
            },
            Source = TranslationSource.WarmCache,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(activeCulture))
            .Returns(loadResult);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.True(initialized);

        translationUpdater.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.WarmCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.WarmCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Never());
    }

    [Fact]
    public async Task SetCultureAsync_WhenSomethingThrows_ShouldLogAndReturnFalse()
    {
        // Arrange
        var newCulture = new CultureInfo("fr-FR");
        var exception = new Exception("Culture not found!");

        translationResolver.Setup(m => m.LoadTranslations(It.IsAny<CultureInfo>()))
            .ThrowsAsync(exception);

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.False(loaded);

        logger.VerifyLog(
            log => log.LogError(exception, "An exception occurred loading culture: {CultureName}", newCulture.Name),
            Times.Once());
    }


    [Fact]
    public async Task SetCultureAsync_WhenCultureCannotBeLoaded_ShouldReturnFalse()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var newCulture = new CultureInfo("fr-FR");

        translationResolver.Setup(m => m.LoadTranslations(newCulture))
            .ReturnsAsync(new TranslationLoadResult()
            {
                Loaded = false,
                Localization = Localization.Invariant,
                Source = TranslationSource.External,
            });

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.False(loaded);
        Assert.Equal(activeCulture, Sut.CurrentCulture);

        logger.VerifyLog(
            log => log.LogWarning("Unable to load culture {CultureName}, no localizations found", newCulture.Name),
            Times.Once());
    }

    [Fact]
    public async Task SetCultureAsync_WhenCultureCanBeLoadedAndLocalTranslationsExist_ShouldUpdateTranslationsAndCulture()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var newCulture = new CultureInfo("fr-FR");

        var externalLoadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = newCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "External Bonjour le monde" },
                },
            },
            Source = TranslationSource.External,
        };

        translationResolver.Setup(m => m.LoadTranslations(newCulture))
            .ReturnsAsync(externalLoadResult);

        var localLoadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = newCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "Bonjour le monde" },
                },
            },
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(newCulture))
            .Returns(localLoadResult);

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.True(loaded);
        Assert.Equal(newCulture, Sut.CurrentCulture);

        currentCultureManager.Verify(
            m => m.SetActiveCulture(newCulture),
            Times.Once());

        logger.VerifyLog(
            log => log.LogDebug("Updated localization culture to {CultureName}", newCulture.Name),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("No internal translations found for culture: {CultureName}, consider adding them as a backup", newCulture.Name),
            Times.Never());

        translationUpdater.Verify(
            m => m.UpdateTranslations(externalLoadResult.Localization, TranslationSource.External),
            Times.Once());

        translationUpdater.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal),
            Times.Once());
    }

    [Fact]
    public async Task SetCultureAsync_WhenCultureCanBeLoadedAndLocalTranslationsDontExist_ShouldUpdateTranslationsAndCulture()
    {
        // Arrange
        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var newCulture = new CultureInfo("fr-FR");

        var externalLoadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = newCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "External Bonjour le monde" },
                },
            },
            Source = TranslationSource.External,
        };

        translationResolver.Setup(m => m.LoadTranslations(newCulture))
            .ReturnsAsync(externalLoadResult);

        var localLoadResult = new TranslationLoadResult()
        {
            Loaded = false,
            Localization = Localization.Invariant,
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(newCulture))
            .Returns(localLoadResult);

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.True(loaded);
        Assert.Equal(newCulture, Sut.CurrentCulture);

        currentCultureManager.Verify(
            m => m.SetActiveCulture(newCulture),
            Times.Once());

        logger.VerifyLog(
            log => log.LogDebug("Updated localization culture to {CultureName}", newCulture.Name),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("No internal translations found for culture: {CultureName}, consider adding them as a backup", newCulture.Name),
            Times.Once());

        translationUpdater.Verify(
            m => m.UpdateTranslations(externalLoadResult.Localization, TranslationSource.External),
            Times.Once());

        translationUpdater.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal),
            Times.Never());
    }

    [Fact]
    public async Task SetCultureAsync_WhenExternalProviderNotEnabledAndLocalTranslationsNotLoaded_ShouldReturnFalseAndNotSetCulture()
    {
        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(false);

        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var newCulture = new CultureInfo("fr-FR");

        var localLoadResult = new TranslationLoadResult()
        {
            Loaded = false,
            Localization = Localization.Invariant,
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(newCulture))
            .Returns(localLoadResult);

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.False(loaded);
        Assert.Equal(activeCulture, Sut.CurrentCulture);
        Assert.NotEqual(newCulture, Sut.CurrentCulture);

        currentCultureManager.Verify(
            m => m.SetActiveCulture(It.IsAny<CultureInfo>()),
            Times.Never);

        logger.VerifyLog(
            log => log.LogDebug("Updated localization culture to {CultureName}", newCulture.Name),
            Times.Never());
    }

    [Fact]
    public async Task SetCultureAsync_WhenExternalProviderNotEnabledAndLocalTranslationsLoaded_ShouldReturnTrue()
    {
        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(false);

        var activeCulture = new CultureInfo("it-IT");

        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(activeCulture);

        var newCulture = new CultureInfo("fr-FR");

        var localLoadResult = new TranslationLoadResult()
        {
            Loaded = true,
            Localization = new Localization()
            {
                CultureInfo = newCulture,
                Translations = new Dictionary<string, string>()
                {
                    { "KeyOne", "Bonjour le monde" },
                },
            },
            Source = TranslationSource.Internal,
        };

        translationResolver.Setup(m => m.LoadLocalTranslations(newCulture))
            .Returns(localLoadResult);

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.True(loaded);
        Assert.Equal(newCulture, Sut.CurrentCulture);

        currentCultureManager.Verify(
            m => m.SetActiveCulture(newCulture),
            Times.Once());

        logger.VerifyLog(
            log => log.LogDebug("Updated localization culture to {CultureName}", newCulture.Name),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("No internal translations found for culture: {CultureName}, consider adding them as a backup", newCulture.Name),
            Times.Never);

        translationUpdater.Verify(
            m => m.UpdateTranslations(It.IsAny<Localization>(), TranslationSource.External),
            Times.Never());

        translationUpdater.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal),
            Times.Once());
    }

    #endregion Tests
}


using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Collections;

namespace Mocale.UnitTests.Managers;

[Collection(CollectionNames.ThreadCultureTests)]
public class LocalizationManagerTests : FixtureBase<ILocalizationManager>, IDisposable
{
    #region Setup

    private readonly Mock<ICurrentCultureManager> currentCultureManager = new();
    private readonly Mock<IConfigurationManager<IMocaleConfiguration>> configurationManager = new();
    private readonly Mock<ILogger<LocalizationManager>> logger = new();
    private readonly Mock<IMocaleConfiguration> mocaleConfiguration = new();
    private readonly Mock<ITranslationResolver> translationResolver = new();
    private readonly Mock<IInternalTranslatorManager> internalTranslatorManager = new();

    public LocalizationManagerTests()
    {
        mocaleConfiguration.SetupGet(m => m.UseExternalProvider)
            .Returns(true);

        configurationManager.SetupGet(m => m.Configuration)
            .Returns(mocaleConfiguration.Object);
    }

    ~LocalizationManagerTests()
    {
        ReleaseUnmanagedResources();
    }

    private static void ReleaseUnmanagedResources()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = null;
        CultureInfo.DefaultThreadCurrentUICulture = null;

    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    public override ILocalizationManager CreateSystemUnderTest()
    {
        return new LocalizationManager(
            currentCultureManager.Object,
            configurationManager.Object,
            logger.Object,
            translationResolver.Object,
            internalTranslatorManager.Object);
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.WarmCache, true),
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.ColdCache, true),
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.Internal, true),
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal, true),
            Times.Once());

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(It.IsAny<Localization>(), TranslationSource.External, true),
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal, true),
            Times.Once());

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(externalLoadResult.Localization, TranslationSource.External, true),
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.Internal, true),
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.ColdCache, true),
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(loadResult.Localization, TranslationSource.WarmCache, true),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Loaded local translations from source: {TranslationSource}", TranslationSource.WarmCache),
            Times.Once());

        logger.VerifyLog(
            log => log.LogInformation("External translations can be updated, checking for newer copy..."),
            Times.Never());
    }

    [Fact]
    public async Task InitializeAsync_WhenInitialized_ShouldSetThreadCulturesToActiveCulture()
    {
        // Arrange
        currentCultureManager.Setup(m => m.GetActiveCulture())
            .Returns(new CultureInfo("it-IT"));

        translationResolver.Setup(m => m.LoadLocalTranslations(new CultureInfo("it-IT")))
            .Returns(new TranslationLoadResult()
            {
                Loaded = true,
                Localization = new Localization()
                {
                    CultureInfo = new CultureInfo("it-IT"),
                    Translations = new Dictionary<string, string>()
                    {
                        { "Hello", "Ciao!" }
                    }
                },
                Source = TranslationSource.WarmCache,
            });

        // This doesn't seem to work when tests run in parallel...
        // Assert.NotEqual(new CultureInfo("it-IT"), Thread.CurrentThread.CurrentCulture);
        // Assert.NotEqual(new CultureInfo("it-IT"), Thread.CurrentThread.CurrentUICulture);
        // Assert.NotEqual(new CultureInfo("it-IT"), CultureInfo.CurrentCulture);
        // Assert.NotEqual(new CultureInfo("it-IT"), CultureInfo.CurrentUICulture);

        // Act
        var initialized = await Sut.Initialize();

        // Assert
        Assert.True(initialized);

        Assert.Equivalent(new CultureInfo("it-IT"), Thread.CurrentThread.CurrentCulture);
        Assert.Equivalent(new CultureInfo("it-IT"), Thread.CurrentThread.CurrentUICulture);
        Assert.Equivalent(new CultureInfo("it-IT"), CultureInfo.CurrentCulture);
        Assert.Equivalent(new CultureInfo("it-IT"), CultureInfo.CurrentUICulture);
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
    public async Task SetCultureAsync_WhenCultureCannotBeLoadedAndInternalTranslationsDontExist_ShouldReturnFalse()
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

        translationResolver.Setup(m => m.LoadLocalTranslations(newCulture))
            .Returns(new TranslationLoadResult()
            {
                Loaded = false,
                Localization = Localization.Invariant,
                Source = TranslationSource.Internal,
            });

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.False(loaded);
        Assert.Equal(activeCulture, Sut.CurrentCulture);

        logger.VerifyLog(
            log => log.LogWarning("Unable to load culture {CultureName}, no localizations found", newCulture.Name),
            Times.Once);

        logger.VerifyLog(
            log => log.LogWarning("Unable to load culture {CultureName} from external provider", newCulture.Name),
            Times.Once);

        logger.VerifyLog(
            log => log.LogInformation("No internal translations found for culture: {CultureName}, consider adding them as a backup", newCulture.Name),
            Times.Once);

        internalTranslatorManager.Verify(m => m.UpdateTranslations(
            It.IsAny<Localization>(),
            It.IsAny<TranslationSource>(),
            It.IsAny<bool>()),
            Times.Never);

        currentCultureManager.Verify(m => m.SetActiveCulture(It.IsAny<CultureInfo>()),
            Times.Never);

        Assert.Equal(activeCulture, Sut.CurrentCulture);

        internalTranslatorManager.Verify(m => m.RaisePropertyChanged(null), Times.Never);
    }

    [Fact]
    public async Task SetCultureAsync_WhenExternalTranslationCannotBeLoadedButInternalTranslationsExist_ShouldReturnTrue()
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

        translationResolver.Setup(m => m.LoadLocalTranslations(newCulture))
            .Returns(new TranslationLoadResult()
            {
                Loaded = true,
                Source = TranslationSource.Internal,
                Localization = new Localization()
                {
                    CultureInfo = new CultureInfo("fr-FR"),
                    Translations = new Dictionary<string, string>()
                    {
                        { "KeyOne", "Bonjour madame" },
                    }
                },
            });

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.True(loaded);
        Assert.Equal(newCulture, Sut.CurrentCulture);

        internalTranslatorManager.Verify(m => m.UpdateTranslations(
            It.Is<Localization>(l =>
                l.Translations.Count == 1 &&
                l.Translations.ContainsKey("KeyOne") &&
                l.Translations["KeyOne"] == "Bonjour madame" &&
                l.CultureInfo.Name == "fr-FR"),
            TranslationSource.Internal,
            false),
            Times.Once);

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(It.IsAny<Localization>(), TranslationSource.External, false),
            Times.Never);

        internalTranslatorManager.Verify(m => m.RaisePropertyChanged(null), Times.Once);
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(externalLoadResult.Localization, TranslationSource.External, false),
            Times.Once());

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal, false),
            Times.Once());

        internalTranslatorManager.Verify(m => m.RaisePropertyChanged(null), Times.Once);
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(externalLoadResult.Localization, TranslationSource.External, false),
            Times.Once());

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal, false),
            Times.Never());

        internalTranslatorManager.Verify(m => m.RaisePropertyChanged(null), Times.Once);
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
            Times.Never);

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(It.IsAny<Localization>(), TranslationSource.External, false),
            Times.Never);

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(It.IsAny<Localization>(), TranslationSource.Internal, false),
            Times.Never);

        internalTranslatorManager.Verify(m => m.RaisePropertyChanged(null), Times.Never);
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

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(It.IsAny<Localization>(), TranslationSource.External, false),
            Times.Never);

        internalTranslatorManager.Verify(
            m => m.UpdateTranslations(localLoadResult.Localization, TranslationSource.Internal, false),
            Times.Once);

        internalTranslatorManager.Verify(m => m.RaisePropertyChanged(null), Times.Once);
    }

    [Fact]
    public async Task SetCultureAsync_WhenLoadSuccessful_ShouldUpdateThreadCultures()
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

        // This doesn't seem to work when tests run in parallel...
        // Assert.NotEqual(new CultureInfo("fr-FR"), Thread.CurrentThread.CurrentCulture);
        // Assert.NotEqual(new CultureInfo("fr-FR"), Thread.CurrentThread.CurrentUICulture);
        // Assert.NotEqual(new CultureInfo("fr-FR"), CultureInfo.CurrentCulture);
        // Assert.NotEqual(new CultureInfo("fr-FR"), CultureInfo.CurrentUICulture);

        // Act
        var loaded = await Sut.SetCultureAsync(newCulture);

        // Assert
        Assert.True(loaded);
        Assert.Equal(newCulture, Sut.CurrentCulture);

        Assert.Equivalent(new CultureInfo("fr-FR"), Thread.CurrentThread.CurrentCulture);
        Assert.Equivalent(new CultureInfo("fr-FR"), Thread.CurrentThread.CurrentUICulture);
        Assert.Equivalent(new CultureInfo("fr-FR"), CultureInfo.CurrentCulture);
        Assert.Equivalent(new CultureInfo("fr-FR"), CultureInfo.CurrentUICulture);
    }

    #endregion Tests
}


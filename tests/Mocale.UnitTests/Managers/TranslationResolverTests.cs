using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale.UnitTests.Managers;

public class TranslationResolverTests : FixtureBase<ITranslationResolver>
{
    #region Setup

    private readonly Mock<ICacheUpdateManager> cacheUpdateManager = new();
    private readonly Mock<IExternalLocalizationProvider> externalLocalizationProvider = new();
    private readonly Mock<IInternalLocalizationProvider> internalLocalizationProvider = new();
    private readonly Mock<ILocalisationCacheManager> localisationCacheManager = new();
    private readonly Mock<ILogger<TranslationResolver>> logger = new();

    public override ITranslationResolver CreateSystemUnderTest()
    {
        return new TranslationResolver(
            cacheUpdateManager.Object,
            externalLocalizationProvider.Object,
            internalLocalizationProvider.Object,
            localisationCacheManager.Object,
            logger.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public async Task LoadTranslations_WhenCacheIsUpToDateAndCachedValuesExist_ShouldReturnCachedTranslations()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("es-ES")))
            .Returns(false);

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string> { { "Hello", "Hola" } });

        // Act
        var result = await Sut.LoadTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.WarmCache, result.Source);
        Assert.Equal(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string> { { "Hello", "Hola" } }, result.Localization.Translations);
    }

    [Fact]
    public async Task LoadTranslations_WhenCacheExpiredAndExternalFails_ShouldLoadFailed()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("es-ES")))
            .Returns(true);

        externalLocalizationProvider.Setup(m => m.GetValuesForCultureAsync(It.IsAny<CultureInfo>()))
            .ReturnsAsync(new ExternalLocalizationResult { Success = false, Localizations = null });

        // Act
        var result = await Sut.LoadTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.False(result.Loaded);
        Assert.Equal(TranslationSource.External, result.Source);
        Assert.Equivalent(Localization.Invariant, result.Localization);

        logger.VerifyLog(log => log.LogDebug(
                "Updating translations for culture: {CultureName} from external provider",
                "es-ES"),
            Times.Once);

        logger.VerifyLog(log => log.LogWarning(
                "No external translations were loaded for culture: {CultureName}",
                "es-ES"),
            Times.Once);
    }

    [Fact]
    public async Task LoadTranslations_WhenCacheExpiredAndExternalReturnsNothing_ShouldLoadFailed()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("es-ES")))
            .Returns(true);

        externalLocalizationProvider.Setup(m => m.GetValuesForCultureAsync(It.IsAny<CultureInfo>()))
            .ReturnsAsync(new ExternalLocalizationResult { Success = true, Localizations = null });

        // Act
        var result = await Sut.LoadTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.False(result.Loaded);
        Assert.Equal(TranslationSource.External, result.Source);
        Assert.Equivalent(Localization.Invariant, result.Localization);

        logger.VerifyLog(log => log.LogDebug(
                "Updating translations for culture: {CultureName} from external provider",
                "es-ES"),
            Times.Once);

        logger.VerifyLog(log => log.LogWarning(
                "No external translations were loaded for culture: {CultureName}",
                "es-ES"),
            Times.Once);
    }

    [Fact]
    public async Task LoadTranslations_ExternalReturnsLocalizationAndSaveFails_ShouldLog()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("es-ES")))
            .Returns(true);

        externalLocalizationProvider.Setup(m => m.GetValuesForCultureAsync(It.IsAny<CultureInfo>()))
            .ReturnsAsync(new ExternalLocalizationResult
            {
                Success = true, Localizations = new Dictionary<string, string> { { "Hello", "Hola" } }
            });

        localisationCacheManager.Setup(m =>
                m.SaveCachedLocalizations(It.IsAny<CultureInfo>(), It.IsAny<Dictionary<string, string>>()))
            .Returns(false);

        // Act
        var result = await Sut.LoadTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.External, result.Source);
        Assert.Equivalent(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string> { { "Hello", "Hola" } }, result.Localization.Translations);

        logger.VerifyLog(log => log.LogDebug(
                "Updating translations for culture: {CultureName} from external provider",
                "es-ES"),
            Times.Once);

        logger.VerifyLog(log => log.LogWarning(
                "Translations were updated for culture: {CultureName}, however they were not saved to cache",
                "es-ES"),
            Times.Once);

        logger.VerifyLog(log => log.LogWarning(
                "No external translations were loaded for culture: {CultureName}",
                "es-ES"),
            Times.Never);
    }

    [Fact]
    public async Task LoadTranslations_WhenLoadingExternalThrows_ShouldCatchLogAndReturnFailure()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(It.IsAny<CultureInfo>()))
            .Returns(true);

        var ex = new Exception("Loading the translations went bodge");

        externalLocalizationProvider.Setup(m => m.GetValuesForCultureAsync(It.IsAny<CultureInfo>()))
            .ThrowsAsync(ex);

        // Act
        var result = await Sut.LoadTranslations(new CultureInfo("en-GB"));

        // Assert
        Assert.False(result.Loaded);
        Assert.Equal(TranslationSource.External, result.Source);
        Assert.Equivalent(Localization.Invariant, result.Localization);

        logger.VerifyLog(log => log.LogError(
                ex,
                "An exception occurred loading translations for culture: {CultureName}",
                "en-GB"),
            Times.Once());
    }

    [Fact]
    public async Task LoadTranslations_WhenCacheIsUpToDateButCachedValuesDontExist_ShouldLoadExternal()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("es-ES")))
            .Returns(false);

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("es-ES")))
            .Returns(() => null);

        externalLocalizationProvider.Setup(m => m.GetValuesForCultureAsync(It.IsAny<CultureInfo>()))
            .ReturnsAsync(new ExternalLocalizationResult
            {
                Success = true, Localizations = new Dictionary<string, string> { { "Hello", "Hola" } }
            });

        localisationCacheManager.Setup(m =>
                m.SaveCachedLocalizations(new CultureInfo("es-ES"),
                    new Dictionary<string, string> { { "Hello", "Hola" } }))
            .Returns(true);

        // Act
        var result = await Sut.LoadTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.External, result.Source);
        Assert.Equivalent(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string> { { "Hello", "Hola" } }, result.Localization.Translations);

        logger.VerifyLog(log => log.LogDebug(
                "Updating translations for culture: {CultureName} from external provider",
                "es-ES"),
            Times.Once);

        logger.VerifyLog(log => log.LogWarning(
                "Translations were updated for culture: {CultureName}, however they were not saved to cache",
                "es-ES"),
            Times.Never);

        logger.VerifyLog(log => log.LogWarning(
                "No external translations were loaded for culture: {CultureName}",
                "es-ES"),
            Times.Never);
    }

    [Fact]
    public async Task LoadTranslations_CacheExpired_ShouldLoadAndSaveExternalTranslations()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("es-ES")))
            .Returns(true);

        externalLocalizationProvider.Setup(m => m.GetValuesForCultureAsync(It.IsAny<CultureInfo>()))
            .ReturnsAsync(new ExternalLocalizationResult
            {
                Success = true, Localizations = new Dictionary<string, string> { { "Hello", "Hola" } }
            });

        localisationCacheManager.Setup(m =>
                m.SaveCachedLocalizations(new CultureInfo("es-ES"),
                    new Dictionary<string, string> { { "Hello", "Hola" } }))
            .Returns(true);

        // Act
        var result = await Sut.LoadTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.External, result.Source);
        Assert.Equivalent(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string> { { "Hello", "Hola" } }, result.Localization.Translations);

        logger.VerifyLog(log => log.LogDebug(
                "Updating translations for culture: {CultureName} from external provider",
                "es-ES"),
            Times.Once);

        logger.VerifyLog(log => log.LogWarning(
                "Translations were updated for culture: {CultureName}, however they were not saved to cache",
                "es-ES"),
            Times.Never);

        logger.VerifyLog(log => log.LogWarning(
                "No external translations were loaded for culture: {CultureName}",
                "es-ES"),
            Times.Never);
    }

    [Fact]
    public void LoadLocalTranslations_WhenInternalTranslationsExistButCacheDoesnt_ShouldReturnInternalTranslations()
    {
        // Arrange
        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("fr-FR")))
            .Returns(new Dictionary<string, string>() { { "Hello", "Bonjour" }, });

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("fr-FR")))
            .Returns(() => null);

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("fr-FR"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.Internal, result.Source);
        Assert.Equal(new CultureInfo("fr-FR"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>() { { "Hello", "Bonjour" }, }, result.Localization.Translations);
    }

    [Fact]
    public void LoadLocalTranslations_WhenWarmCacheAndNoInternalTranslations_ShouldReturnCachedTranslations()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("fr-FR")))
            .Returns(false);

        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("fr-FR")))
            .Returns(() => null);

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("fr-FR")))
            .Returns(new Dictionary<string, string>() { { "Hello", "Bonjour" }, });

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("fr-FR"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.WarmCache, result.Source);
        Assert.Equal(new CultureInfo("fr-FR"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>() { { "Hello", "Bonjour" }, }, result.Localization.Translations);
    }

    [Fact]
    public void LoadLocalTranslations_WhenColdCacheAndNoInternalTranslations_ShouldReturnCachedTranslations()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(new CultureInfo("fr-FR")))
            .Returns(true);

        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("fr-FR")))
            .Returns(() => null);

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("fr-FR")))
            .Returns(new Dictionary<string, string>() { { "Hello", "Bonjour" }, });

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("fr-FR"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.ColdCache, result.Source);
        Assert.Equal(new CultureInfo("fr-FR"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>() { { "Hello", "Bonjour" }, }, result.Localization.Translations);
    }

    [Fact]
    public void LoadLocalTranslations_WhenInternalAndCacheTranslationsAreNull_ShouldReturnFailedResult()
    {
        // Arrange
        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("fr-FR")))
            .Returns(() => null);

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("fr-FR")))
            .Returns(() => null);

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("fr-FR"));

        // Assert
        Assert.False(result.Loaded);
        Assert.Equal(TranslationSource.Internal, result.Source);
        Assert.Equivalent(Localization.Invariant, result.Localization);
    }

    [Fact]
    public void LoadLocalTranslations_WhenBothInternalAndCacheTranslationsExistWhenCacheWarm_ShouldMergeValues()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(It.IsAny<CultureInfo>()))
            .Returns(false);

        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" }
            });

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" }
            });

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.WarmCache, result.Source);
        Assert.Equal(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "Hello", "Hola" }
        }, result.Localization.Translations);
    }

    [Fact]
    public void LoadLocalTranslations_WhenBothInternalAndCacheTranslationsExistWhenCacheCold_ShouldMergeValues()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(It.IsAny<CultureInfo>()))
            .Returns(true);

        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" }
            });

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" }
            });

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.ColdCache, result.Source);
        Assert.Equal(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "Hello", "Hola" }
        }, result.Localization.Translations);
    }

    [Fact]
    public void LoadLocalTranslations_WhenCacheContainsMoreThanInternalTranslations_ShouldMergeValues()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(It.IsAny<CultureInfo>()))
            .Returns(false);

        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" }
            });

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" },
                { "Chicken", "Pollo" },
            });

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.WarmCache, result.Source);
        Assert.Equal(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "Hello", "Hola" },
            { "Chicken", "Pollo" },
        }, result.Localization.Translations);
    }

    [Fact]
    public void LoadLocalTranslations_WhenCacheOverwritesInternalTranslations_ShouldMergeValuesAndPreferCache()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(It.IsAny<CultureInfo>()))
            .Returns(false);

        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" }
            });

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola!!!" },
            });

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.WarmCache, result.Source);
        Assert.Equal(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "Hello", "Hola!!!" },
        }, result.Localization.Translations);
    }

    [Fact]
    public void LoadLocalTranslations_WhenInternalTranslationsContainsValuesNotInCache_ShouldWarnAboutMissingValues()
    {
        // Arrange
        cacheUpdateManager.Setup(m => m.CanUpdateCache(It.IsAny<CultureInfo>()))
            .Returns(false);

        internalLocalizationProvider.Setup(m => m.GetValuesForCulture(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" },
                { "City", "Ciudad" },
                { "WelcomeToMyShop", "Bienvenido a mi tienda" },
            });

        localisationCacheManager.Setup(m => m.GetCachedLocalizations(new CultureInfo("es-ES")))
            .Returns(new Dictionary<string, string>()
            {
                { "Hello", "Hola" },
            });

        // Act
        var result = Sut.LoadLocalTranslations(new CultureInfo("es-ES"));

        // Assert
        Assert.True(result.Loaded);
        Assert.Equal(TranslationSource.WarmCache, result.Source);
        Assert.Equal(new CultureInfo("es-ES"), result.Localization.CultureInfo);
        Assert.Equivalent(new Dictionary<string, string>()
        {
            { "Hello", "Hola" },
            { "City", "Ciudad" },
            { "WelcomeToMyShop", "Bienvenido a mi tienda" },
        }, result.Localization.Translations);

        logger.VerifyLog(log => log.LogInformation(
            "The following keys were present in the local translations but not in the cache: {AddedKeys}",
            new List<string>() { "City","WelcomeToMyShop" }),
            Times.Once);
    }

    #endregion Tests
}

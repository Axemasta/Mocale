using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Managers;

namespace Mocale.UnitTests.Managers;

public class LocalizationManagerTests : FixtureBase<ILocalizationManager>
{
    #region Setup

    private readonly Mock<ICurrentCultureManager> currentCultureManager;
    private readonly Mock<ILogger<LocalizationManager>> logger;
    private readonly Mock<ITranslationResolver> translationResolver;
    private readonly Mock<ITranslationUpdater> translationUpdater;

    public LocalizationManagerTests()
    {
        currentCultureManager = new Mock<ICurrentCultureManager>();
        logger = new Mock<ILogger<LocalizationManager>>();
        translationResolver = new Mock<ITranslationResolver>();
        translationUpdater = new Mock<ITranslationUpdater>();
    }

    public override ILocalizationManager CreateSystemUnderTest()
    {
        return new LocalizationManager(
            currentCultureManager.Object,
            logger.Object,
            translationResolver.Object,
            translationUpdater.Object
            );
    }

    #endregion Setup

    #region Tests

    [Fact]
    public async Task CurrentCulture_ShouldGetSetInConstructor()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task CurrentCulture_WhenCultureChangesShouldSave_ShouldReturnConfigurationDefault()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Initialize_WhenExceptionIsThrown_ShouldLogAndReturnFalse()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsArentLoaded_ShouldReturnFalse()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsAreHot_ShouldUpdateAndReturnTrue()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsAreCold_ShouldUpdateTranslationsAndTriggerUpdateAndReturnTrue()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Initialize_WhenLocalTranslationsAreInternal_ShouldUpdateTranslationsAndTriggerUpdateAndReturnTrue()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Initialize_WhenTranslationsCanBeUpdatedAndExternalFailsToLoad_ShouldLogFailedUpdatedAndReturnTrue()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Initialize_WhenTranslationsCanBeUpdatedAndExternalLoads_ShouldUpdateAndReturnTrue()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task SetCultureAsync_WhenSomethingThrows_ShouldLogAndReturnFalse()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }


    [Fact]
    public async Task SetCultureAsync_WhenCultureCannotBeLoaded_ShouldReturnFalse()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task SetCultureAsync_WhenCultureCanBeLoaded_ShouldUpdateTranslationsAndCulture()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    #endregion Tests
}


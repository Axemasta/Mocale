using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Managers;

namespace Mocale.UnitTests.Managers;

public class CurrentCultureManagerTests : FixtureBase<ICurrentCultureManager>
{
    #region Setup

    private readonly Mock<IConfigurationManager<IMocaleConfiguration>> mocaleConfigurationManager;
    private readonly Mock<ILogger<CurrentCultureManager>> logger;
    private readonly Mock<IPreferences> preferences;

    public CurrentCultureManagerTests()
    {
        mocaleConfigurationManager = new Mock<IConfigurationManager<IMocaleConfiguration>>();
        logger = new Mock<ILogger<CurrentCultureManager>>();
        preferences = new Mock<IPreferences>();
    }

    public override ICurrentCultureManager CreateSystemUnderTest()
    {
        return new CurrentCultureManager(
            mocaleConfigurationManager.Object,
            logger.Object,
            preferences.Object);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void GetActiveCulture_WhenCultureSaveIsDisabled_ShouldReturnConfigDefault()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GetActiveCulture_WhenPreferencesIsEmpty_ShouldSaveConfigDefault()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GetActiveCulture_WhenPreferencesStringCantBeParsed_ShouldReturnConfigDefault()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GetActiveCulture_WhenPreferencesStringCanBeParsed_ShouldReturnLastUsedCulture()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void SetActiveCulture_WhenSaveCultureChangedDisabled_ShouldDoNothing()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void SetActiveCulture_WhenSaveCultureChangedEnabled_ShouldSerializeCultureAndSaveToPreferences()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    #endregion Tests
}


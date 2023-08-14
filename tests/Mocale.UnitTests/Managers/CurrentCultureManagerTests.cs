using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale.UnitTests.Managers;

public class CurrentCultureManagerTests : FixtureBase<ICurrentCultureManager>
{
    #region Setup

    private const string CorrectPreferencesKey = "Mocale_LastUsedCulture";

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
        var defaultCulture = new CultureInfo("en-GB");

        mocaleConfigurationManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                SaveCultureChanged = false,
                DefaultCulture = defaultCulture,
            });

        // Act
        var activeCulture = Sut.GetActiveCulture();

        // Assert
        Assert.Equal(defaultCulture, activeCulture);

        preferences.Verify(
            m => m.Get(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null),
            Times.Never());

        preferences.Verify(
            m => m.Set(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null),
            Times.Never());
    }

    [Fact]
    public void GetActiveCulture_WhenPreferencesIsEmpty_ShouldSaveConfigDefault()
    {
        // Arrange
        var defaultCulture = new CultureInfo("en-GB");

        mocaleConfigurationManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                SaveCultureChanged = true,
                DefaultCulture = defaultCulture,
            });

        preferences.Setup(m => m.Get(CorrectPreferencesKey, string.Empty, null))
            .Returns(string.Empty);

        // Act
        var activeCulture = Sut.GetActiveCulture();

        // Assert
        Assert.Equal(defaultCulture, activeCulture);

        preferences.Verify(
            m => m.Get(
                It.Is<string>(s => s.Equals(CorrectPreferencesKey, StringComparison.Ordinal)),
                string.Empty,
                null),
            Times.Once());

        preferences.Verify(
            m => m.Set(
                It.Is<string>(s => s.Equals(CorrectPreferencesKey, StringComparison.Ordinal)),
                "en-GB",
                null),
            Times.Once());

        logger.VerifyLog(
            log => log.LogTrace("Setting Last Used Culture as: {DefaultCulture}", defaultCulture),
            Times.Once());
    }

    [Fact]
    public void GetActiveCulture_WhenPreferencesStringCantBeParsed_ShouldReturnConfigDefault()
    {
        // Arrange
        var defaultCulture = new CultureInfo("en-GB");
        var invalidCultureString = "ThIs-IsNot_A-CulTuR3";

        mocaleConfigurationManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                SaveCultureChanged = true,
                DefaultCulture = defaultCulture,
            });

        preferences.Setup(m => m.Get(CorrectPreferencesKey, string.Empty, null))
            .Returns(invalidCultureString);

        // Act
        var activeCulture = Sut.GetActiveCulture();

        // Assert
        Assert.Equal(defaultCulture, activeCulture);

        preferences.Verify(
            m => m.Get(
                It.Is<string>(s => s.Equals(CorrectPreferencesKey, StringComparison.Ordinal)),
                string.Empty,
                null),
            Times.Once());

        preferences.Verify(
            m => m.Set(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null),
            Times.Never());

        logger.VerifyLog(
            log => log.LogWarning("Unable to parse culture from preferences: {LastUsedCulture}", invalidCultureString),
            Times.Once());
    }

    [Fact]
    public void GetActiveCulture_WhenPreferencesStringCanBeParsed_ShouldReturnLastUsedCulture()
    {
        // Arrange
        var defaultCulture = new CultureInfo("en-GB");
        var validCultureString = "fr-FR";

        mocaleConfigurationManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                SaveCultureChanged = true,
                DefaultCulture = defaultCulture,
            });

        preferences.Setup(m => m.Get(CorrectPreferencesKey, string.Empty, null))
            .Returns(validCultureString);

        // Act
        var activeCulture = Sut.GetActiveCulture();

        // Assert
        Assert.Equal(new CultureInfo(validCultureString), activeCulture);

        preferences.Verify(
            m => m.Get(
                It.Is<string>(s => s.Equals(CorrectPreferencesKey, StringComparison.Ordinal)),
                string.Empty,
                null),
            Times.Once());

        preferences.Verify(
            m => m.Set(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null),
            Times.Never());

        logger.VerifyLog(
            log => log.LogWarning("Unable to parse culture from preferences: {LastUsedCulture}", It.IsAny<object?[]>()),
            Times.Never());

        logger.VerifyLog(
            log => log.LogTrace("Setting Last Used Culture as: {DefaultCulture}", It.IsAny<CultureInfo>()),
            Times.Never());
    }

    [Fact]
    public void SetActiveCulture_WhenSaveCultureChangedDisabled_ShouldDoNothing()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        mocaleConfigurationManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                SaveCultureChanged = false,
            });

        // Act
        Sut.SetActiveCulture(culture);

        // Assert
        preferences.Verify(
            m => m.Set(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null),
            Times.Never());
    }

    [Fact]
    public void SetActiveCulture_WhenSaveCultureChangedEnabled_ShouldSerializeCultureAndSaveToPreferences()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        mocaleConfigurationManager.SetupGet(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                SaveCultureChanged = true,
            });

        // Act
        Sut.SetActiveCulture(culture);

        // Assert
        preferences.Verify(
            m => m.Set(
                It.Is<string>(s => s.Equals(CorrectPreferencesKey, StringComparison.Ordinal)),
                It.Is<string>(s => s.Equals("en-US", StringComparison.Ordinal)),
                null),
            Times.Once());
    }

    #endregion Tests
}


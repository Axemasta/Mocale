using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Mocale.Abstractions;
using Mocale.Cache.SQLite.Abstractions;
using Mocale.Cache.SQLite.Entities;
using Mocale.Cache.SQLite.Managers;
using Mocale.Cache.SQLite.Models;

namespace Mocale.UnitTests.Cache;

public class SqlCacheUpdateManagerTests : FixtureBase<ICacheUpdateManager>
{
    #region Setup

    private readonly Mock<ICacheRepository> cacheRepository = new();
    private readonly Mock<ILogger<SqlCacheUpdateManager>> logger = new();
    private readonly Mock<IConfigurationManager<ISqliteConfig>> configurationManager = new();
    private readonly FakeTimeProvider timeProvider = new(new DateTimeOffset(2025, 2, 26, 22, 16, 0, TimeSpan.FromHours(0)));

    public override ICacheUpdateManager CreateSystemUnderTest()
    {
        return new SqlCacheUpdateManager(
            cacheRepository.Object,
            logger.Object,
            configurationManager.Object,
            timeProvider);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void CanUpdateCache_WhenCultureNotCached_ShouldReturnTrue()
    {
        // Arrange
        cacheRepository.Setup(m => m.GetItem(new CultureInfo("en-GB")))
            .Returns(() => null);

        // Act
        var canUpdate = Sut.CanUpdateCache(new CultureInfo("en-GB"));

        // Assert
        Assert.True(canUpdate);
    }

    [Fact]
    public void CanUpdateCache_WhenCultureCachedAndOutsideWarmCacheWindow_ShouldReturnTrue()
    {
        // Arrange
        configurationManager.Setup(m => m.Configuration)
            .Returns(new SqliteConfig()
            {
                UpdateInterval = TimeSpan.FromDays(1),
            });

        cacheRepository.Setup(m => m.GetItem(new CultureInfo("en-GB")))
            .Returns(new UpdateHistoryItem()
            {
                Id = 0,
                CultureName = "en-GB",
                LastUpdated = new DateTime(2025, 1, 1, 12, 0, 0)
            });

        // Act
        var canUpdate = Sut.CanUpdateCache(new CultureInfo("en-GB"));

        // Assert
        Assert.True(canUpdate);
    }

    [Fact]
    public void CanUpdateCache_WhenCultureCachedAndInsideWarmCacheWindow_ShouldReturnFalse()
    {
        // Arrange
        configurationManager.Setup(m => m.Configuration)
            .Returns(new SqliteConfig()
            {
                UpdateInterval = TimeSpan.FromDays(1),
            });

        cacheRepository.Setup(m => m.GetItem(new CultureInfo("en-GB")))
            .Returns(new UpdateHistoryItem()
            {
                Id = 0,
                CultureName = "en-GB",
                LastUpdated = new DateTime(2025, 2, 26, 17, 40, 0)
            });

        // Act
        var canUpdate = Sut.CanUpdateCache(new CultureInfo("en-GB"));

        // Assert
        Assert.False(canUpdate);
    }

    [Fact]
    public void SetCacheUpdated_ShouldReturnTrue_WhenAddOrUpdateItemSucceeds()
    {
        // Arrange
        cacheRepository.Setup(m => m.AddOrUpdateItem(
                It.Is<CultureInfo>(c => c.Name == "fr-FR"),
                It.Is<DateTime>(d => d == new DateTime(2025, 2, 26, 22, 16, 0))))
            .Returns(true);

        // Act
        var frFrUpdated = Sut.SetCacheUpdated(new CultureInfo("fr-FR"));

        // Assert
        Assert.True(frFrUpdated);
    }

    [Fact]
    public void SetCacheUpdated_ShouldReturnFalse_WhenAddOrUpdateItemFails()
    {
        // Arrange
        cacheRepository.Setup(m => m.AddOrUpdateItem(
                It.Is<CultureInfo>(c => c.Name == "en-GB"),
                It.Is<DateTime>(d => d == new DateTime(2025, 2, 26, 22, 16, 0))))
            .Returns(false);

        // Act
        var enGbUpdated = Sut.SetCacheUpdated(new CultureInfo("en-GB"));

        // Assert
        Assert.False(enGbUpdated);
    }

    [Fact]
    public void ClearCache_WithCulture_ShouldLogWarning_WhenDeleteItemFails()
    {
        // Arrange
        cacheRepository.Setup(repo => repo.DeleteItem(new CultureInfo("en-US")))
            .Returns(false);

        // Act
        Sut.ClearCache(new CultureInfo("en-US"));

        // Assert
        logger.VerifyLog(log => log.LogWarning("Unable to delete cache for culture: {CultureName}", "en-US"), Times.Once);
        logger.VerifyLog(log => log.LogTrace("Deleted update cache for culture: {CultureName}", It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ClearCache_WithCulture_ShouldLogTrace_WhenDeleteItemSucceeds()
    {
        // Arrange
        cacheRepository.Setup(repo => repo.DeleteItem(new CultureInfo("en-US"))).Returns(true);

        // Act
        Sut.ClearCache(new CultureInfo("en-US"));

        // Assert
        logger.VerifyLog(log => log.LogWarning("Unable to delete cache for culture: {CultureName}", It.IsAny<string>()), Times.Never);
        logger.VerifyLog(log => log.LogTrace("Deleted update cache for culture: {CultureName}", "en-US"), Times.Once);
    }

    [Fact]
    public void ClearCache_ShouldLogWarning_WhenDeleteAllFails()
    {
        // Arrange
        cacheRepository.Setup(repo => repo.DeleteAll())
            .Returns(false);

        // Act
        Sut.ClearCache();

        // Assert
        logger.VerifyLog(log => log.LogWarning("Unable to delete cache for all cultures"), Times.Once);
        logger.VerifyLog(log => log.LogTrace("Deleted update cache for all cultures"), Times.Never);
    }

    [Fact]
    public void ClearCache_ShouldLogTrace_WhenDeleteAllSucceeds()
    {
        // Arrange
        cacheRepository.Setup(repo => repo.DeleteAll())
            .Returns(true);

        // Act
        Sut.ClearCache();

        // Assert
        logger.VerifyLog(log => log.LogWarning("Unable to delete cache for all cultures"), Times.Never);
        logger.VerifyLog(log => log.LogTrace("Deleted update cache for all cultures"), Times.Once);
    }

    #endregion Tests
}

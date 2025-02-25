using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Mocale.Cache;

namespace Mocale.UnitTests.Cache;

public class InMemoryCacheManagerTests : FixtureBase, IDisposable
{
    #region Setup

    private readonly MemoryCache memoryCache = new(new MemoryCacheOptions());

    public override object CreateSystemUnderTest()
    {
        return new InMemoryCacheManager(memoryCache);
    }

    public void Dispose()
    {
        memoryCache?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void CanUpdateCache_WhenKeyNotInCache_ReturnsTrue()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        // Act
        var result = GetSut<InMemoryCacheManager>().CanUpdateCache(culture);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanUpdateCache_WhenKeyInCache_ReturnsFalse()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        memoryCache.Set(culture, new Dictionary<string, string>()
        {
            { "KeyOne", "ValueOne" }
        });

        // Act
        var result = GetSut<InMemoryCacheManager>().CanUpdateCache(culture);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ClearCache_RemovesCacheEntry()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        memoryCache.Set(culture, new Dictionary<string, string>());

        // Act
        GetSut<InMemoryCacheManager>().ClearCache(culture);

        // Assert
        Assert.False(memoryCache.TryGetValue(culture, out _));
    }

    [Fact]
    public void ClearCache_CallsClearMethod()
    {
        // Arrange

        // Act
        GetSut<InMemoryCacheManager>().ClearCache();

        // Assert
    }

    [Fact]
    public void GetCachedLocalizations_WhenInCache_ReturnsDictionary()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        var localizations = new Dictionary<string, string> { { "Hello", "Hola" } };
        memoryCache.Set(culture, localizations);

        // Act
        var result = GetSut<InMemoryCacheManager>().GetCachedLocalizations(culture);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Hola", result["Hello"]);
    }

    [Fact]
    public void GetCachedLocalizations_WhenNotInCache_ReturnsNull()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        // Act
        var result = GetSut<InMemoryCacheManager>().GetCachedLocalizations(culture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SaveCachedLocalizations_AddsEntryToCache()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        var localizations = new Dictionary<string, string> { { "Hello", "Hola" } };

        // Act
        var result = GetSut<InMemoryCacheManager>().SaveCachedLocalizations(culture, localizations);

        // Assert
        Assert.True(result);
        Assert.True(memoryCache.TryGetValue(culture, out var cachedData));
        Assert.Equal(localizations, cachedData);
    }

    [Fact]
    public void SetCacheUpdated_AlwaysReturnsTrue()
    {
        // Arrange
        var culture = new CultureInfo("en-US");

        // Act
        var result = GetSut<InMemoryCacheManager>().SetCacheUpdated(culture);

        // Assert
        Assert.True(result);
    }

    #endregion Tests
}

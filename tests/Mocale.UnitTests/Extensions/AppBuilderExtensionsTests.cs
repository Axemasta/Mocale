using Microsoft.Extensions.Caching.Memory;
using Mocale.Abstractions;
using Mocale.Cache;
using Mocale.Cache.SQLite;
using Mocale.Cache.SQLite.Managers;
using Mocale.Exceptions;
using Mocale.Managers;
using Mocale.Models;
using Mocale.Providers;
using Mocale.UnitTests.TestUtils;

namespace Mocale.UnitTests.Extensions;

public class AppBuilderExtensionsTests
{
    [Fact]
    public void UseMocale_WhenNoLocalProviderRegistered_ShouldThrow()
    {
        // Arrange
        var mauiAppBuilder = MauiApp.CreateBuilder();

        // Act
        var ex = Assert.Throws<InitializationException>(() => mauiAppBuilder.UseMocale(builder => { }));

        // Assert
        Assert.Equal("No local provider has been registered, please call either UseAppResources or UseEmbeddedResources in order to use mocale", ex.Message);
    }

    [Fact]
    public void UseMocale_UseExternalProviderWithoutRegistration_ShouldThrow()
    {
        // Arrange
        var mauiAppBuilder = MauiApp.CreateBuilder();

        // Act & Assert
        var exception = Assert.Throws<InitializationException>(() =>
            mauiAppBuilder.UseMocale(builder =>
            {
                builder.UseEmbeddedResources(config => { });
                builder.WithConfiguration(config =>
                {
                    config.UseExternalProvider = true;
                });
                builder.ExternalProviderRegistered = false;
            })
        );

        Assert.Contains("No external provider was registered when mocale was configured to use one. Please register an external provider or set UseExternalProvider to false.", exception.Message);
    }

    [Fact]
    public void UseMocale_NotUsingExternalProvider_ShouldRegisterInactiveProvider()
    {
        // Arrange
        var mauiAppBuilder = MauiApp.CreateBuilder();

        // Act
        mauiAppBuilder.UseMocale(builder =>
        {
            builder.UseEmbeddedResources(config => { });
            builder.WithConfiguration(config =>
            {
                config.UseExternalProvider = false;
            });
        });

        // Assert
        mauiAppBuilder.Services.ShouldContainImplementation<IExternalLocalizationProvider, InactiveExternalLocalizationProvider>();
    }

    [Fact]
    public void UseMocale_WithCacheProvider_ShouldNotRegisterInMemoryCache()
    {
        // Arrange
        var mauiAppBuilder = MauiApp.CreateBuilder();

        // Act
        mauiAppBuilder.UseMocale(builder =>
        {
            builder.UseEmbeddedResources(config => { });
            builder.WithConfiguration(config =>
            {
                config.UseExternalProvider = false;
            });
            builder.UseSqliteCache(Mock.Of<IFileSystem>(), config => { });
        });

        // Assert
        mauiAppBuilder.Services.ShouldNotContainImplementation<ILocalisationCacheManager, InMemoryCacheManager>();
        mauiAppBuilder.Services.ShouldNotContainImplementation<ICacheUpdateManager, InMemoryCacheManager>();

        mauiAppBuilder.Services.ShouldContainImplementation<ILocalisationCacheManager, LocalisationCacheManager>();
        mauiAppBuilder.Services.ShouldContainImplementation<ICacheUpdateManager, SqlCacheUpdateManager>();
    }

    [Fact]
    public void UseMocale_WithNoCacheProvider_ShouldRegisterInMemoryCache()
    {
        // Arrange
        var mauiAppBuilder = MauiApp.CreateBuilder();

        // Act
        mauiAppBuilder.UseMocale(builder =>
        {
            builder.UseEmbeddedResources(config => { });
            builder.WithConfiguration(config =>
            {
                config.UseExternalProvider = false;
            });
        });

        // Assert
        mauiAppBuilder.Services.ShouldContainImplementation<ILocalisationCacheManager, InMemoryCacheManager>();
        mauiAppBuilder.Services.ShouldContainImplementation<ICacheUpdateManager, InMemoryCacheManager>();
    }
}

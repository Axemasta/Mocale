using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Exceptions;
using Mocale.Helper;
using Mocale.Models;
using Mocale.Parsers;
using Mocale.Providers;
using Mocale.Providers.Azure.Blob;
using Mocale.Providers.Azure.Blob.Models;
using Mocale.Providers.GitHub.Raw.Models;
using Mocale.UnitTests.TestHelpers;

namespace Mocale.UnitTests.Extensions;

public class MocaleBuilderExtensionsTests : FixtureBase<MocaleBuilder>
{
    #region Setup

    private readonly MauiAppBuilder appBuilder = MauiApp.CreateBuilder();

    public override MocaleBuilder CreateSystemUnderTest()
    {
        return new MocaleBuilder()
        {
            AppBuilder = appBuilder,
            ConfigurationManager = new Mocale.Managers.ConfigurationManager<Abstractions.IMocaleConfiguration>(new MocaleConfiguration())
        };
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void UseAppResources()
    {
        // Arrange

        // Act
        Sut.UseAppResources(config =>
        {
            config.AppResourcesType = typeof(AppResourcesConfig);
        });

        // Assert
        Assert.True(Sut.LocalProviderRegistered);
        Assert.Equal("AppResourceProvider", Sut.LocalProviderName);
        Assert.Equal(LocaleResourceType.Resx, Sut.ConfigurationManager.Configuration.ResourceType);

        Sut.AppBuilder.Services.ShouldHaveRegisteredService<IConfigurationManager<IAppResourcesConfig>>(ServiceLifetime.Singleton);
        Sut.AppBuilder.Services.ShouldHaveRegisteredService<IInternalLocalizationProvider, AppResourceProvider>(ServiceLifetime.Singleton);
    }

    [Fact]
    public void UseEmbeddedResources()
    {
        // Arrange

        // Act
        Sut.UseEmbeddedResources(config => { });

        // Assert
        Assert.True(Sut.LocalProviderRegistered);
        Assert.Equal("EmbeddedResourceProvider", Sut.LocalProviderName);
        Assert.Equal(LocaleResourceType.Json, Sut.ConfigurationManager.Configuration.ResourceType);

        Sut.AppBuilder.Services.ShouldHaveRegisteredService<IConfigurationManager<IEmbeddedResourcesConfig>>(ServiceLifetime.Singleton);
        Sut.AppBuilder.Services.ShouldHaveRegisteredService<IInternalLocalizationProvider, EmbeddedResourceProvider>(ServiceLifetime.Singleton);
    }

    [Fact]
    public void RegisterLocalProvider_WhenTypeNotAssignableToInterface_ShouldThrow()
    {
        // Arrange

        // Act
        var ex = Assert.Throws<InitializationException>(() => MocaleBuilderExtensions.RegisterLocalProvider(Sut, typeof(string)));

        // Assert
        Assert.Equal("The provider is not of type IInternalLocalizationProvider (actual: String)", ex.Message);
    }

    [Fact]
    public void RegisterLocalProvider_WhenLocalProviderAlreadyRegistered_ShouldThrow()
    {
        // Arrange
        MocaleBuilderExtensions.RegisterLocalProvider(Sut, typeof(EmbeddedResourceProvider));

        // Act
        var ex = Assert.Throws<InitializationException>(() => MocaleBuilderExtensions.RegisterLocalProvider(Sut, typeof(AppResourceProvider)));

        // Assert
        Assert.Equal("The following local provider was already registered: EmbeddedResourceProvider", ex.Message);
    }

    [Fact]
    public void RegisterLocalProvider()
    {
        // Arrange

        // Act
        MocaleBuilderExtensions.RegisterLocalProvider(Sut, typeof(EmbeddedResourceProvider));

        // Assert
        Assert.True(Sut.LocalProviderRegistered);
        Assert.Equal("EmbeddedResourceProvider", Sut.LocalProviderName);
    }

    [Fact]
    public void RegisterExternalProvider_WhenTypeNotAssignableToInterface_ShouldThrow()
    {
        // Arrange

        // Act
        var ex = Assert.Throws<InitializationException>(() => MocaleBuilderExtensions.RegisterExternalProvider(Sut, typeof(string), new BlobStorageConfig()));

        // Assert
        Assert.Equal("The provider is not of type IExternalLocalizationProvider (actual: String)", ex.Message);
    }

    [Fact]
    public void RegisterExternalProvider_WhenExternalProviderAlreadyRegistered_ShouldThrow()
    {
        // Arrange
        MocaleBuilderExtensions.RegisterExternalProvider(Sut, typeof(BlobLocalizationProvider), new BlobStorageConfig());

        // Act
        var ex = Assert.Throws<InitializationException>(() => MocaleBuilderExtensions.RegisterExternalProvider(Sut, typeof(Mocale.Providers.GitHub.Raw.GitHubRawProvider), new GithubRawConfig()));

        // Assert
        Assert.Equal("The following external provider was already registered: BlobLocalizationProvider", ex.Message);
    }

    [Fact]
    public void RegisterExternalProvider()
    {
        // Arrange

        // Act
        MocaleBuilderExtensions.RegisterExternalProvider(Sut, typeof(BlobLocalizationProvider), new BlobStorageConfig()
        {
            ResourceFileDetails = new JsonResourceFileDetails(),
        });

        // Assert
        Assert.True(Sut.ExternalProviderRegistered);
        Assert.Equal("BlobLocalizationProvider", Sut.ExternalProviderName);

        Sut.AppBuilder.Services.ShouldHaveRegisteredService<IConfigurationManager<BlobStorageConfig>>(ServiceLifetime.Singleton);
        Sut.AppBuilder.Services.ShouldHaveRegisteredService<IConfigurationManager<IExternalProviderConfiguration>>(ServiceLifetime.Singleton);
        Sut.AppBuilder.Services.ShouldContainImplementation<IExternalFileNameHelper, ExternalJsonFileNameHelper>();
        Sut.AppBuilder.Services.ShouldContainImplementation<ILocalizationParser, JsonLocalizationParser>();
    }

    [Fact]
    public void RegisterExternalResourceFileTypeResources_WhenResourceTypeUnsupported_ShouldThrow()
    {
        // Arrange
        var externalConfigMock = new Mock<IExternalProviderConfiguration>();
        externalConfigMock.Setup(m => m.ResourceFileDetails.ResourceType)
            .Returns((LocaleResourceType)13576);

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => MocaleBuilderExtensions.RegisterExternalResourceFileTypeResources(Sut, externalConfigMock.Object));

        // Assert
        Assert.Equal("Invalid value for LocaleResourceType: 13576", ex.Message);
    }

    [Fact]
    public void RegisterExternalResourceFileTypeResources_WhenResourceTypeJson_ShouldRegisterJsonHandlers()
    {
        // Arrange
        var externalConfigMock = new Mock<IExternalProviderConfiguration>();
        externalConfigMock.Setup(m => m.ResourceFileDetails.ResourceType)
            .Returns(LocaleResourceType.Json);

        // Act
        MocaleBuilderExtensions.RegisterExternalResourceFileTypeResources(Sut, externalConfigMock.Object);

        // Assert
        Sut.AppBuilder.Services.ShouldContainImplementation<IExternalFileNameHelper, ExternalJsonFileNameHelper>();
        Sut.AppBuilder.Services.ShouldContainImplementation<ILocalizationParser, JsonLocalizationParser>();
    }

    [Fact]
    public void RegisterExternalResourceFileTypeResources_WhenResourceTypeResx_ShouldRegisterResxHandlers()
    {
        // Arrange
        var externalConfigMock = new Mock<IExternalProviderConfiguration>();
        externalConfigMock.Setup(m => m.ResourceFileDetails.ResourceType)
            .Returns(LocaleResourceType.Resx);

        // Act
        MocaleBuilderExtensions.RegisterExternalResourceFileTypeResources(Sut, externalConfigMock.Object);

        // Assert
        Sut.AppBuilder.Services.ShouldContainImplementation<IExternalFileNameHelper, ExternalResxFileNameHelper>();
        Sut.AppBuilder.Services.ShouldContainImplementation<ILocalizationParser, ResxLocalizationParser>();
    }

    #endregion Tests
}

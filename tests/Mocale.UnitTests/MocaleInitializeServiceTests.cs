using System.Globalization;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale.UnitTests;

public class MocaleInitializeServiceTests : FixtureBase
{
    #region Setup

    public override object CreateSystemUnderTest()
    {
        return new MocaleInitializeService();
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void Initialize_ShouldSetupLocator_AndInitializeServices()
    {
        // Arrange
        var appBuilder = MauiApp.CreateBuilder();

        var localizationManager = new Mock<ILocalizationManager>();
        var translatorManager = new Mock<ITranslatorManager>();
        var configManager = new Mock<IConfigurationManager<IMocaleConfiguration>>();

        configManager.Setup(m => m.Configuration)
            .Returns(new MocaleConfiguration()
            {
                ResourceType = LocaleResourceType.Json,
                DefaultCulture = new CultureInfo("fr-FR"),
                ShowMissingKeys = false,
                NotFoundSymbol = "__",
                UseExternalProvider = false,
                SaveCultureChanged = false,
            });

        appBuilder.Services.AddSingleton(localizationManager.Object);
        appBuilder.Services.AddSingleton(translatorManager.Object);
        appBuilder.Services.AddSingleton(configManager.Object);

        var serviceProvider = appBuilder.Services.BuildServiceProvider();

        var sut = GetSut<MocaleInitializeService>();

        // Act
        sut.Initialize(serviceProvider);

        // Assert
        Assert.NotNull(MocaleLocator.MocaleConfiguration);
        Assert.Equal(LocaleResourceType.Json, MocaleLocator.MocaleConfiguration.ResourceType);
        Assert.Equal(new CultureInfo("fr-FR"), MocaleLocator.MocaleConfiguration.DefaultCulture);
        Assert.False(MocaleLocator.MocaleConfiguration.ShowMissingKeys);
        Assert.Equal("__", MocaleLocator.MocaleConfiguration.NotFoundSymbol);
        Assert.False(MocaleLocator.MocaleConfiguration.UseExternalProvider);
        Assert.False(MocaleLocator.MocaleConfiguration.SaveCultureChanged);

        Assert.NotNull(MocaleLocator.TranslatorManager);
        Assert.Equal(translatorManager.Object, MocaleLocator.TranslatorManager);

        localizationManager.Verify(m => m.Initialize(), Times.Once);
    }

    #endregion Tests
}

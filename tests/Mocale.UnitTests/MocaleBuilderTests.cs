using System.ComponentModel;
using System.Globalization;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale.UnitTests;

public class MocaleBuilderTests
{
    [Fact]
    public void WithConfiguration_WhenConfigurationIsNull_ShouldThrow()
    {
        // Arrange
        var mauiAppBuilder = MauiApp.CreateBuilder();
        var mocaleBuilder = new MocaleBuilder()
        {
            AppBuilder = mauiAppBuilder,
            ConfigurationManager = new ConfigurationManager<IMocaleConfiguration>(Mock.Of<IMocaleConfiguration>()),
        };

        // Act
        var ex = Assert.Throws<InvalidCastException>(() => mocaleBuilder.WithConfiguration(_ => { }));

        // Assert
        Assert.Equal("Unable to cast IMocaleConfiguration as MocaleConfiguration", ex.Message);
    }

    [Fact]
    public void WithConfiguration_WhenCalled_ShouldAllowConfiguration()
    {
        // Arrange
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

        var config = new MocaleConfiguration();
        Assert.Equal(LocaleResourceType.Resx, config.ResourceType);
        Assert.Equal(new CultureInfo("en-GB"), config.DefaultCulture);
        Assert.True(config.ShowMissingKeys);
        Assert.Equal("$", config.NotFoundSymbol);
        Assert.True(config.UseExternalProvider);
        Assert.True(config.SaveCultureChanged);
        Assert.True(config.EnumBehavior.UseAttribute);
        Assert.Equal(typeof(DescriptionAttribute), config.EnumBehavior.LocalizeAttribute);
        Assert.Equal(nameof(DescriptionAttribute.Description), config.EnumBehavior.AttributePropertyName);
        Assert.Empty(config.EnumBehavior.OverrideRules);

        var mauiAppBuilder = MauiApp.CreateBuilder();
        var mocaleBuilder = new MocaleBuilder()
        {
            AppBuilder = mauiAppBuilder,
            ConfigurationManager = new ConfigurationManager<IMocaleConfiguration>(config),
        };

        // Act
        mocaleBuilder.WithConfiguration(builderConfig =>
        {
            builderConfig.ResourceType = LocaleResourceType.Json;
            builderConfig.DefaultCulture = new CultureInfo("fr-FR");
            builderConfig.ShowMissingKeys = false;
            builderConfig.NotFoundSymbol = "__";
            builderConfig.UseExternalProvider = false;
            builderConfig.SaveCultureChanged = false;
            builderConfig.EnumBehavior = new LocalizeEnumBehavior()
            {
                AttributePropertyName = "MyProperty",
                UseAttribute = false,
                LocalizeAttribute = typeof(BindableAttribute),
            };
        });

        // Assert
        Assert.Equal(LocaleResourceType.Json, config.ResourceType);
        Assert.Equal(new CultureInfo("fr-FR"), config.DefaultCulture);
        Assert.False(config.ShowMissingKeys);
        Assert.Equal("__", config.NotFoundSymbol);
        Assert.False(config.UseExternalProvider);
        Assert.False(config.SaveCultureChanged);
        Assert.False(config.EnumBehavior.UseAttribute);
        Assert.Equal(typeof(BindableAttribute), config.EnumBehavior.LocalizeAttribute);
        Assert.Equal("MyProperty", config.EnumBehavior.AttributePropertyName);
    }
}

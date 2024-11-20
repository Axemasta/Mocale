using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Managers;
using Mocale.Models;
using Mocale.UnitTests.Collections;

namespace Mocale.UnitTests.Extensions;

[Collection(CollectionNames.MocaleLocatorTests)]
public class BindableObjectExtensionTests
{
    private readonly TranslatorManager translatorManager;

    private readonly Mock<ILogger<TranslatorManager>> logger = new();
    private readonly Mock<IConfigurationManager<IMocaleConfiguration>> configurationManager = new();

    public BindableObjectExtensionTests()
    {
        var config = new MocaleConfiguration()
        {
            ResourceType = LocaleResourceType.Json,
            DefaultCulture = new CultureInfo("en-GB"),
            NotFoundSymbol = "$",
            SaveCultureChanged = false,
            ShowMissingKeys = true,
            UseExternalProvider = false,
        };

        configurationManager.Setup(m => m.Configuration)
            .Returns(config);

        translatorManager = new TranslatorManager(logger.Object, configurationManager.Object);

        MocaleLocator.SetInstance(translatorManager);
    }

    [Fact]
    public void SetTranslation_WhenTranslationKeyDoesNotExist_ShouldSetNotFoundKey()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "MapplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        label.SetTranslation(Label.TextProperty, "ApplicationTitle");

        // Assert
        Assert.Equal("$ApplicationTitle$", label.Text);
    }

    [Fact]
    public void SetTranslation_WhenTranslationKeyExists_ShouldSetTranslation()
    {
        // Arrange
        var localization = new Localization()
        {
            CultureInfo = new CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "ApplicationTitle", "Mocale!"}
            }
        };

        translatorManager.UpdateTranslations(localization, TranslationSource.Internal);

        var label = new Label();

        // Act
        label.SetTranslation(Label.TextProperty, "ApplicationTitle");

        // Assert
        Assert.Equal("Mocale!", label.Text);
    }
}

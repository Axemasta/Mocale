using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Extensions;
using Mocale.Managers;
using Mocale.Models;
using Mocale.Testing;
using Mocale.UnitTests.Collections;

namespace Mocale.UnitTests.Extensions;

[Collection(CollectionNames.MocaleLocatorTests)]
public class TranslatorManagerExtensionsTests : FixtureBase<ITranslatorManager>
{
    #region Setup

    private readonly Mock<IMocaleConfiguration> mocaleConfiguration = new();
    private readonly TranslatorManagerProxy translatorManager = new();

    public override ITranslatorManager CreateSystemUnderTest()
    {
        MocaleLocator.MocaleConfiguration = mocaleConfiguration.Object;

        return translatorManager;
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void TranslateEnum_WhenEnumHasNoAttributes_ShouldUseValueToString()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.EnumBehavior)
            .Returns(new LocalizeEnumBehavior());

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Value1", "Value One"},
                { "Value2", "Value Two"},
                { "Value13", "Value Three"}
            }
        }, TranslationSource.Internal);

        // Act
        var translation = Sut.TranslateEnum(NoAttributeEnum.Value2);

        // Assert
        Assert.Equal("Value Two", translation);
    }

    [Fact]
    public void TranslateEnum_WhenEnumHasAttributesButDisabledViaConfig_ShouldUseValueToString()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.EnumBehavior)
            .Returns(new LocalizeEnumBehavior()
            {
                UseAttribute = false,
            });

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Value1Key", "Value One"},
                { "Value2Key", "Value Two"},
                { "Value3Key", "Value Three"}
            }
        }, TranslationSource.Internal);

        // Act
        var translation = Sut.TranslateEnum(DescriptionAttributeEnum.Value2);

        // Assert
        Assert.Equal("Value2", translation);
    }

    [Fact]
    public void TranslateEnum_WhenDefaultEnumConfig_ShouldUseDescriptionAttribute()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.EnumBehavior)
            .Returns(new LocalizeEnumBehavior());

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "Value1Key", "Value One"},
                { "Value2Key", "Value Two"},
                { "Value3Key", "Value Three"}
            }
        }, TranslationSource.Internal);

        // Act
        var translation = Sut.TranslateEnum(DescriptionAttributeEnum.Value3);

        // Assert
        Assert.Equal("Value Three", translation);
    }

    [Fact]
    public void TranslateEnum_WhenCustomEnumConfig_ShouldUseConfigAttribute()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.EnumBehavior)
            .Returns(new LocalizeEnumBehavior()
            {
                LocalizeAttribute = typeof(LocalizationKeyAttribute),
                AttributePropertyName = nameof(LocalizationKeyAttribute.Key),
                UseAttribute = true
            });

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "RightKeyOne", "Custom Value One"},
                { "RightKeyTwo", "Custom Value Two"},
                { "RightKeyThree", "Custom Value Three"}
            }
        }, TranslationSource.Internal);

        // Act
        var translation = Sut.TranslateEnum(CustomAttributeEnum.Value1);

        // Assert
        Assert.Equal("Custom Value One", translation);
    }

    [Fact]
    public void TranslateEnum_WhenSpecificEnumRule_ShouldUseRuleAttribute()
    {
        // Arrange
        mocaleConfiguration.SetupGet(m => m.EnumBehavior)
            .Returns(new LocalizeEnumBehavior()
            {
                LocalizeAttribute = typeof(DescriptionAttribute),
                AttributePropertyName = nameof(DescriptionAttribute.Description),
                UseAttribute = true,
                OverrideRules =
                {
                    {
                        typeof(CustomAttributeEnum), new LocalizeEnumRule()
                        {
                            LocalizeAttribute = typeof(LocalizationKeyAttribute),
                            AttributePropertyName = nameof(LocalizationKeyAttribute.Key),
                            UseAttribute = true,
                        }
                    }
                }
            });

        translatorManager.UpdateTranslations(new Localization(new CultureInfo("en-GB"))
        {
            Translations = new Dictionary<string, string>()
            {
                { "RightKeyOne", "Custom Value One"},
                { "RightKeyTwo", "Custom Value Two"},
                { "RightKeyThree", "Custom Value Three"}
            }
        }, TranslationSource.Internal);

        // Act
        var translation = Sut.TranslateEnum(CustomAttributeEnum.Value2);

        // Assert
        Assert.Equal("Custom Value Two", translation);
    }

    #endregion Tests

    #region Test Data

    private enum NoAttributeEnum
    {
        Value1,
        Value2,
        Value3
    }

    private enum DescriptionAttributeEnum
    {
        [Description("Value1Key")]
        Value1,

        [Description("Value2Key")]
        Value2,

        [Description("Value3Key")]
        Value3
    }

    private enum CustomAttributeEnum
    {
        [Description("WRONG KEY 1")]
        [LocalizationKey("RightKeyOne")]
        Value1,

        [Description("WRONG KEY 2")]
        [LocalizationKey("RightKeyTwo")]
        Value2,

        [Description("WRONG KEY 3")]
        [LocalizationKey("RightKeyThree")]
        Value3,
    }

    [AttributeUsage(AttributeTargets.Field)]
    private class LocalizationKeyAttribute(string key) : Attribute
    {
        public string Key { get; } = key;
    }

    #endregion Test Data
}

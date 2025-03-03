using System.ComponentModel;
using Mocale.Helper;
using Mocale.Models;

namespace Mocale.UnitTests.Helpers;

public class EnumTranslationKeyHelperTests
{
    private enum TestEnum
    {
        ValueWithoutAttribute,

        [Description("LocalizedName")]
        [CustomTranslationKey("BingBongKey")]
        ValueWithAttribute
    }

    [AttributeUsage(AttributeTargets.Field)]
    private sealed class CustomTranslationKeyAttribute(string translationKey) : Attribute
    {
        public string TranslationKey { get; } = translationKey;
    }

    [Fact]
    public void GetTranslationKey_WhenEnumHasNoAttribute_ShouldSetEnumStringValue()
    {
        // Arrange
        var config = new MocaleConfiguration();
        var helper = new EnumTranslationKeyHelper(config);

        // Act
        var result = helper.GetTranslationKey(TestEnum.ValueWithoutAttribute);

        // Assert
        Assert.Equal("ValueWithoutAttribute", result);
    }

    [Fact]
    public void GetTranslationKey_WhenEnumHasNoOverrideRules_ShouldGetKeyFromGlobalConfig()
    {
        // Arrange
        var config = new MocaleConfiguration
        {
            EnumBehavior = new LocalizeEnumBehavior
            {
                UseAttribute = true,
                LocalizeAttribute = typeof(DescriptionAttribute),
                AttributePropertyName = nameof(DescriptionAttribute.Description),
            }
        };

        var helper = new EnumTranslationKeyHelper(config);

        // Act
        var result = helper.GetTranslationKey(TestEnum.ValueWithAttribute);

        // Assert
        Assert.Equal("LocalizedName", result);
    }

    [Fact]
    public void GetTranslationKey_WhenConfigShouldNotUseAttribute_ShouldGetKeyFromToString()
    {
        // Arrange
        var config = new MocaleConfiguration
        {
            EnumBehavior = new LocalizeEnumBehavior
            {
                UseAttribute = false,
            }
        };

        var helper = new EnumTranslationKeyHelper(config);

        // Act
        var result = helper.GetTranslationKey(TestEnum.ValueWithAttribute);

        // Assert
        Assert.Equal("ValueWithAttribute", result);
    }

    [Fact]
    public void GetTranslationKey_WhenEnumHasOverrideRules_ShouldUseSpecificRulesForEnum()
    {
        // Arrange
        var config = new MocaleConfiguration
        {
            EnumBehavior = new LocalizeEnumBehavior
            {
                UseAttribute = false,
                LocalizeAttribute = typeof(Attribute),
                AttributePropertyName = "???",
            }
        };

        config.EnumBehavior.OverrideRules.Add(typeof(TestEnum), new LocalizeEnumRule()
        {
            UseAttribute = true,
            LocalizeAttribute = typeof(CustomTranslationKeyAttribute),
            AttributePropertyName = nameof(CustomTranslationKeyAttribute.TranslationKey),
        });

        var helper = new EnumTranslationKeyHelper(config);

        // Act
        var result = helper.GetTranslationKey(TestEnum.ValueWithAttribute);

        // Assert
        Assert.Equal("BingBongKey", result);
    }
}

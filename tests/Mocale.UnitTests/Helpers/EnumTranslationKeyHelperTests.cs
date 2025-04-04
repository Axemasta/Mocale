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

        // Act
        var result = EnumTranslationKeyHelper.GetTranslationKey(TestEnum.ValueWithoutAttribute, new LocalizeEnumBehavior());

        // Assert
        Assert.Equal("ValueWithoutAttribute", result);
    }

    [Fact]
    public void GetTranslationKey_WhenEnumHasNoOverrideRules_ShouldGetKeyFromGlobalConfig()
    {
        // Arrange
        var behavior = new LocalizeEnumBehavior
        {
            UseAttribute = true,
            LocalizeAttribute = typeof(DescriptionAttribute),
            AttributePropertyName = nameof(DescriptionAttribute.Description),
        };

        // Act
        var result = EnumTranslationKeyHelper.GetTranslationKey(TestEnum.ValueWithAttribute, behavior);

        // Assert
        Assert.Equal("LocalizedName", result);
    }

    [Fact]
    public void GetTranslationKey_WhenConfigShouldNotUseAttribute_ShouldGetKeyFromToString()
    {
        // Arrange
        var behavior = new LocalizeEnumBehavior
        {
            UseAttribute = false,
        };

        // Act
        var result = EnumTranslationKeyHelper.GetTranslationKey(TestEnum.ValueWithAttribute, behavior);

        // Assert
        Assert.Equal("ValueWithAttribute", result);
    }

    [Fact]
    public void GetTranslationKey_WhenEnumHasOverrideRules_ShouldUseSpecificRulesForEnum()
    {
        // Arrange
        var behavior = new LocalizeEnumBehavior
        {
            UseAttribute = false,
            LocalizeAttribute = typeof(Attribute),
            AttributePropertyName = "???",
        };

        behavior.OverrideRules.Add(typeof(TestEnum), new LocalizeEnumRule()
        {
            UseAttribute = true,
            LocalizeAttribute = typeof(CustomTranslationKeyAttribute),
            AttributePropertyName = nameof(CustomTranslationKeyAttribute.TranslationKey),
        });

        // Act
        var result = EnumTranslationKeyHelper.GetTranslationKey(TestEnum.ValueWithAttribute, behavior);

        // Assert
        Assert.Equal("BingBongKey", result);
    }
}

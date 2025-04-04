using Mocale.Extensions;
using Mocale.Models;

namespace Mocale.UnitTests.Extensions;

public class LocalizeEnumBehaviorExtensionTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShouldLocalizeEnum_WhenEnumValueIsNotInOverrideRules_ShouldReturnGlobalUseAttribute(bool useAttribute)
    {
        // Arrange
        var localizeEnumBehavior = new LocalizeEnumBehavior()
        {
            UseAttribute = useAttribute
        };

        var enumValue = EnumOne.A;

        // Act
        var result = localizeEnumBehavior.ShouldLocalizeEnum(enumValue);

        // Assert
        Assert.Equal(useAttribute, result);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void ShouldLocalizeEnum_WhenEnumValueIsInOverrideRules_ShouldReturnRuleUseAttribute(bool globalUseAttribute, bool ruleUseAttribute)
    {
        // Arrange
        var localizeEnumBehavior = new LocalizeEnumBehavior()
        {
            UseAttribute = globalUseAttribute,
            OverrideRules =
            {
                {
                    typeof(EnumTwo), new LocalizeEnumRule()
                    {
                        UseAttribute = ruleUseAttribute,
                    }
                }
            }
        };

        var enumValue = EnumTwo.Y;

        // Act
        var result = localizeEnumBehavior.ShouldLocalizeEnum(enumValue);

        // Assert
        Assert.Equal(ruleUseAttribute, result);
    }

    private enum EnumOne
    {
        A,
        B,
        C
    }

    private enum EnumTwo
    {
        X,
        Y,
        Z
    }
}

namespace Mocale.Helper;

internal class EnumTranslationKeyHelper(IMocaleConfiguration mocaleConfiguration)
{
    public string GetTranslationKey(Enum @enum)
    {
        string? translationKey = null;

        var behavior = mocaleConfiguration.EnumBehavior;

        if (behavior.OverrideRules.TryGetValue(@enum.GetType(), out var rule))
        {
            translationKey = GetKey(@enum, rule.UseAttribute, rule.LocalizeAttribute, rule.AttributePropertyName);
        }
        else
        {
            translationKey = GetKey(@enum, behavior.UseAttribute, behavior.LocalizeAttribute, behavior.AttributePropertyName);
        }

        translationKey ??= @enum.ToString();

        return translationKey;
    }

    private static string? GetKey(Enum enumValue, bool useAttribute, Type localizeAttribute, string propertyName)
    {
        if (useAttribute)
        {
            return enumValue.GetAttributeValue(localizeAttribute, propertyName);
        }
        else
        {
            return enumValue.ToString();
        }
    }
}

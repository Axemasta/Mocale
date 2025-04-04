namespace Mocale.Helper;

internal static class EnumTranslationKeyHelper
{
    public static string GetTranslationKey(Enum @enum, LocalizeEnumBehavior behavior)
    {
        return behavior.OverrideRules.TryGetValue(@enum.GetType(), out var rule)
            ? GetKey(@enum, rule.UseAttribute, rule.LocalizeAttribute, rule.AttributePropertyName)
            : GetKey(@enum, behavior.UseAttribute, behavior.LocalizeAttribute, behavior.AttributePropertyName);
    }

    private static string GetKey(Enum enumValue, bool useAttribute, Type localizeAttribute, string propertyName)
    {
        if (useAttribute)
        {
            return enumValue.GetAttributeValue(localizeAttribute, propertyName) ?? enumValue.ToString();
        }
        else
        {
            return enumValue.ToString();
        }
    }
}

using System.Diagnostics;

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
internal static class EnumExtensions
{
    public static string? GetAttributeValue(this Enum enumValue, Type attributeType, string propertyName)
    {
        if (!typeof(Attribute).IsAssignableFrom(attributeType))
        {
            throw new ArgumentException($"Provided type {attributeType.Name} is not an attribute.", nameof(attributeType));
        }

        var attribute = enumValue.GetType()
            .GetField(enumValue.ToString())
            ?.GetCustomAttributes(attributeType, false)
            .FirstOrDefault();

        if (attribute is null)
        {
            Trace.WriteLine($"Attribute {attributeType.Name} not found on {enumValue}.");
            return null;
        }

        var property = attributeType.GetProperty(propertyName);

        if (property is null)
        {
            Trace.WriteLine($"Property '{propertyName}' not found on attribute {attributeType.Name}.");
            return null;
        }

        return property.GetValue(attribute) as string;
    }
}

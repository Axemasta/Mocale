using System.Diagnostics;

namespace Mocale.Extensions;

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

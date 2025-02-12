using System.ComponentModel;

namespace Mocale.Models;

/// <summary>
/// Localize Enum Behavior
/// </summary>
public record LocalizeEnumBehavior
{
    /// <summary>
    /// Whether to get translation key from the enum's attribute. If false it will use ToString().
    /// </summary>
    public bool UseAttribute { get; init; } = true;

    /// <summary>
    /// The attribute to get the translation key from the enum.
    /// </summary>
    public Type LocalizeAttribute { get; init; } = typeof(DescriptionAttribute);

    /// <summary>
    /// The attribute property from which to retrieve the translation key for the enum.
    /// </summary>
    public string AttributePropertyName { get; init; } = nameof(DescriptionAttribute.Description);

    /// <summary>
    /// Enum specific rules, you can override the behavior for specific enum types here if you wish.
    /// </summary>
    public Dictionary<Type, LocalizeEnumRule> OverrideRules { get; } = [];
}

public record LocalizeEnumRule
{
    /// <summary>
    /// Whether to get translation key from the enum's attribute. If false it will use ToString().
    /// </summary>
    public bool UseAttribute { get; init; } = true;

    /// <summary>
    /// The attribute to get the translation key from the enum.
    /// </summary>
    public Type LocalizeAttribute { get; init; } = typeof(DescriptionAttribute);

    /// <summary>
    /// The attribute property from which to retrieve the translation key for the enum.
    /// </summary>
    public string AttributePropertyName { get; init; } = nameof(DescriptionAttribute.Description);
}

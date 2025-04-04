namespace Mocale.Extensions;

internal static class LocalizeEnumBehaviorExtension
{
    public static bool ShouldLocalizeEnum(this LocalizeEnumBehavior localizeEnumBehavior, Enum enumValue)
    {
        return localizeEnumBehavior.OverrideRules.TryGetValue(enumValue.GetType(), out var rule)
            ? rule.UseAttribute
            : localizeEnumBehavior.UseAttribute;
    }
}

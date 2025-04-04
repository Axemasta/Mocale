using Mocale.Helper;

namespace Mocale.Extensions;

/// <summary>
/// Translator Manager Extensions
/// </summary>
public static class TranslatorManagerExtensions
{
    /// <summary>
    /// Translate Enum
    /// </summary>
    /// <param name="translatorManager">The translator manager instance</param>
    /// <param name="enumValue">The enum value to translate</param>
    /// <returns>Translation result</returns>
    public static string TranslateEnum(this ITranslatorManager translatorManager, Enum enumValue)
    {
        var mocaleConfiguration = MocaleLocator.MocaleConfiguration;

        return TranslateEnum(translatorManager, enumValue, mocaleConfiguration.EnumBehavior);
    }

    /// <summary>
    /// Translate Enum
    /// </summary>
    /// <param name="translatorManager">The translator manager instance</param>
    /// <param name="enumValue">The enum value to translate</param>
    /// <param name="localizeEnumBehavior">The localization behavior to use for the enum</param>
    /// <returns></returns>
    public static string TranslateEnum(this ITranslatorManager translatorManager, Enum enumValue, LocalizeEnumBehavior localizeEnumBehavior)
    {
        if (!localizeEnumBehavior.ShouldLocalizeEnum(enumValue))
        {
            return enumValue.ToString();
        }

        var translationKey = EnumTranslationKeyHelper.GetTranslationKey(enumValue, localizeEnumBehavior);

        return translatorManager.Translate(translationKey);
    }
}

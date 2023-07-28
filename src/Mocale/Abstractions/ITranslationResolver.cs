using System.Globalization;
namespace Mocale.Abstractions;

/// <summary>
/// Translation Resolver
/// </summary>
public interface ITranslationResolver
{
    /// <summary>
    /// Load Translations For Given Culture
    /// </summary>
    /// <param name="cultureInfo">The culture to load</param>
    /// <returns>The load result</returns>
    Task<TranslationLoadResult> LoadTranslations(CultureInfo cultureInfo);

    /// <summary>
    /// Load Local Translations For Given Culture
    /// <para>Local is defined as either from cache of on the device</para>
    /// </summary>
    /// <param name="cultureInfo">The culture to load</param>
    /// <returns>The load result</returns>
    TranslationLoadResult LoadLocalTranslations(CultureInfo cultureInfo);
}

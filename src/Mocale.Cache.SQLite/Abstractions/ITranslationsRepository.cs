using System.Globalization;
namespace Mocale.Cache.SQLite.Abstractions;

/// <summary>
/// Translation Repository
/// </summary>
public interface ITranslationsRepository
{
    /// <summary>
    /// Get Translations For Culture
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    Dictionary<string, string>? GetTranslations(CultureInfo cultureInfo);

    /// <summary>
    /// Add Translations For Culture
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <param name="translations"></param>
    /// <returns></returns>
    bool AddTranslations(CultureInfo cultureInfo, Dictionary<string, string> translations);

    /// <summary>
    /// Delete Translations For Culture
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    bool DeleteTranslations(CultureInfo cultureInfo);
}

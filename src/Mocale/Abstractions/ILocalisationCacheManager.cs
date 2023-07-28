using System.Globalization;
namespace Mocale.Abstractions;

/// <summary>
/// Localisation Cache Manager
/// </summary>
public interface ILocalisationCacheManager
{
    /// <summary>
    /// Get Cached Localizations For Culture
    /// </summary>
    /// <param name="cultureInfo">Culture to get cached values for</param>
    /// <returns>Cached localizations if they exist</returns>
    Dictionary<string, string>? GetCachedLocalizations(CultureInfo cultureInfo);

    /// <summary>
    /// Set Cached Localizations For Culture
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <param name="localizations"></param>
    /// <returns></returns>
    bool SaveCachedLocalizations(CultureInfo cultureInfo, Dictionary<string, string> localizations);
}

using System.Globalization;
namespace Mocale.Abstractions;

/// <summary>
/// Cache Update Manager
/// <para>Responsible for tracking when the localization cache has been updated</para>
/// </summary>
public interface ICacheUpdateManager
{
    /// <summary>
    /// Whether the cache for the current culture can be updated.
    /// <para>This is called before checking external providers</para>
    /// </summary>
    /// <param name="cultureInfo">The culture to check whether updates can occur</param>
    /// <returns>True if external provider should be used (cache expired / no localizations for culture)</returns>
    bool CanUpdateCache(CultureInfo cultureInfo);

    /// <summary>
    /// Sets when the cache has been updated for the given culture
    /// </summary>
    /// <param name="cultureInfo">The culture that has just been updated</param>
    /// <returns>Success</returns>
    bool SetCacheUpdated(CultureInfo cultureInfo);

    /// <summary>
    /// Clear the cache for a specific culture
    /// </summary>
    /// <param name="cultureInfo">Culture to clear cache for</param>
    void ClearCache(CultureInfo cultureInfo);

    /// <summary>
    /// Clear the cache for all cultures
    /// </summary>
    void ClearCache();
}

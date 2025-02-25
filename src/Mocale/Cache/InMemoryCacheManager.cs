using System.Globalization;
using Microsoft.Extensions.Caching.Memory;

namespace Mocale.Cache;

internal class InMemoryCacheManager(MemoryCache memoryCache) : ILocalisationCacheManager, ICacheUpdateManager
{
    public bool CanUpdateCache(CultureInfo cultureInfo)
    {
        return !memoryCache.TryGetValue(cultureInfo, out _);
    }

    public void ClearCache(CultureInfo cultureInfo)
    {
        memoryCache.Remove(cultureInfo);
    }

    public void ClearCache()
    {
        memoryCache.Clear();
    }

    public Dictionary<string, string>? GetCachedLocalizations(CultureInfo cultureInfo)
    {
        if (memoryCache.TryGetValue<Dictionary<string, string>>(cultureInfo, out var cachedLocalizations))
        {
            return cachedLocalizations;
        }

        return null;
    }

    public bool SaveCachedLocalizations(CultureInfo cultureInfo, Dictionary<string, string> localizations)
    {
        memoryCache.Set(cultureInfo, localizations);

        return true;
    }

    public bool SetCacheUpdated(CultureInfo cultureInfo)
    {
        return true;
    }
}

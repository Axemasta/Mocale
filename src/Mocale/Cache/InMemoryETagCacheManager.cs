using System.Collections.Concurrent;
using System.Globalization;

namespace Mocale.Cache;

/// <summary>
/// In-memory implementation of IETagCacheManager.
/// For persistent ETag storage, use the SQLite cache package.
/// </summary>
internal class InMemoryETagCacheManager(ILogger<InMemoryETagCacheManager> logger) : IETagCacheManager
{
    private readonly ConcurrentDictionary<string, string> etagCache = new();

    public string? GetETag(CultureInfo cultureInfo)
    {
        var key = cultureInfo.Name;

        if (!etagCache.TryGetValue(key, out var etag))
        {
            logger.LogDebug("No ETag stored for culture {CultureName}", cultureInfo.Name);
            return null;
        }

        logger.LogDebug("Retrieved ETag for culture {CultureName}: {ETag}", cultureInfo.Name, etag);

        return etag;
    }

    public bool SaveETag(CultureInfo cultureInfo, string etag)
    {
        var key = cultureInfo.Name;
        etagCache[key] = etag;

        logger.LogDebug("Saved ETag for culture {CultureName}: {ETag}",
            cultureInfo.Name, etag);

        return true;
    }

    public bool RemoveETag(CultureInfo cultureInfo)
    {
        var key = cultureInfo.Name;
        var removed = etagCache.TryRemove(key, out _);

        if (removed)
        {
            logger.LogDebug("Removed ETag for culture {CultureName}", cultureInfo.Name);
        }
        else
        {
            logger.LogDebug("No ETag to remove for culture {CultureName}", cultureInfo.Name);
        }

        return removed;
    }
}

using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Cache.SQLite.Managers;

internal class EtagCacheManager : IETagCacheManager
{
    public string? GetETag(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }

    public bool SaveETag(CultureInfo cultureInfo, string etag)
    {
        throw new NotImplementedException();
    }

    public bool RemoveETag(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }
}

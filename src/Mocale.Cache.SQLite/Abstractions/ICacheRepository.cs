using System.Globalization;

namespace Mocale.Cache.SQLite.Abstractions;

public interface ICacheRepository
{
    bool AddOrUpdateItem(CultureInfo cultureInfo, DateTime lastUpdated);

    UpdateHistoryItem? GetItem(CultureInfo cultureInfo);

    bool DeleteAll();

    bool DeleteItem(CultureInfo cultureInfo);
}

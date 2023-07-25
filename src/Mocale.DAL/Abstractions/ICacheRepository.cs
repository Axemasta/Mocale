using System.Globalization;

namespace Mocale.DAL.Abstractions;

public interface ICacheRepository
{
    bool AddOrUpdateItem(CultureInfo cultureInfo, DateTime lastUpdated);

    UpdateHistoryItem? GetItem(CultureInfo cultureInfo);

    bool DeleteAll();

    bool DeleteItem(CultureInfo cultureInfo);
}

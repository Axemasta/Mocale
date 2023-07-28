using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Mocale.Cache.SQLite.Repositories;

public class CacheRepository : RepositoryBase, ICacheRepository
{
    #region Constructors

    public CacheRepository(
        IDatabaseConnectionProvider databaseConnectionProvider,
        ILogger<CacheRepository> logger)
        : base(
            databaseConnectionProvider,
            logger)
    {
        Connection.CreateTable<UpdateHistoryItem>();
    }

    #endregion Constructors

    #region Interface Implementations

    public bool AddItem(UpdateHistoryItem updateItem)
    {
        var rows = Connection.Insert(updateItem);

        return rows == 1;
    }

    public bool AddOrUpdateItem(CultureInfo cultureInfo, DateTime lastUpdated)
    {
        var existingItem = Connection.Table<UpdateHistoryItem>()
            .FirstOrDefault(e => e.CultureName == cultureInfo.Name);

        if (existingItem is not null)
        {
            existingItem.LastUpdated = lastUpdated;

            var rows = Connection.Update(existingItem);

            return rows == 1;
        }
        else
        {
            var entity = new UpdateHistoryItem()
            {
                CultureName = cultureInfo.Name,
                LastUpdated = lastUpdated,
            };

            var rows = Connection.Insert(entity);

            return rows == 1;
        }
    }

    public bool DeleteAll()
    {
        var count = Connection.Table<UpdateHistoryItem>()
            .Count();

        var rows = Connection.DeleteAll<UpdateHistoryItem>();

        return rows == count;
    }

    public bool DeleteItem(CultureInfo cultureInfo)
    {
        var itemToDelete = Connection.Table<UpdateHistoryItem>()
            .FirstOrDefault(e => e.CultureName == cultureInfo.Name);

        if (itemToDelete is null)
        {
            return false;
        }

        var rows = Connection.Delete(itemToDelete);

        return rows == 1;
    }

    public UpdateHistoryItem? GetItem(CultureInfo cultureInfo)
    {
        return Connection.Table<UpdateHistoryItem>()
            .FirstOrDefault(e => e.CultureName == cultureInfo.Name);
    }

    public bool UpdateItem(UpdateHistoryItem updateItem)
    {
        var rows = Connection.Update(updateItem);

        return rows == 1;
    }

    #endregion Interface Implementations
}


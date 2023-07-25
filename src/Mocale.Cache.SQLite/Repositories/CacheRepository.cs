using System.Globalization;

namespace Mocale.Cache.SQLite.Repositories;

public class CacheRepository : ICacheRepository
{
    #region Fields

    private readonly SQLiteConnection databaseConnection;

    #endregion Fields

    #region Constructors

    public CacheRepository(IDatabaseConnectionProvider databaseConnectionProvider)
    {
        this.databaseConnection = databaseConnectionProvider.GetDatabaseConnection();

        databaseConnection.CreateTable<UpdateHistoryItem>();
    }

    #endregion Constructors

    #region Interface Implementations

    public bool AddItem(UpdateHistoryItem updateItem)
    {
        var rows = databaseConnection.Insert(updateItem);

        return rows == 1;
    }

    public bool AddOrUpdateItem(CultureInfo cultureInfo, DateTime lastUpdated)
    {
        var existingItem = databaseConnection.Table<UpdateHistoryItem>()
            .FirstOrDefault(e => e.CultureName == cultureInfo.Name);

        if (existingItem is not null)
        {
            existingItem.LastUpdated = lastUpdated;

            var rows = databaseConnection.Update(existingItem);

            return rows == 1;
        }
        else
        {
            var entity = new UpdateHistoryItem()
            {
                CultureName = cultureInfo.Name,
                LastUpdated = lastUpdated,
            };

            var rows = databaseConnection.Insert(entity);

            return rows == 1;
        }
    }

    public bool DeleteAll()
    {
        var count = databaseConnection.Table<UpdateHistoryItem>()
            .Count();

        var rows = databaseConnection.DeleteAll<UpdateHistoryItem>();

        return rows == count;
    }

    public bool DeleteItem(CultureInfo cultureInfo)
    {
        var itemToDelete = databaseConnection.Table<UpdateHistoryItem>()
            .FirstOrDefault(e => e.CultureName == cultureInfo.Name);

        if (itemToDelete is null)
        {
            return false;
        }

        var rows = databaseConnection.Delete(itemToDelete);

        return rows == 1;
    }

    public UpdateHistoryItem? GetItem(CultureInfo cultureInfo)
    {
        return databaseConnection.Table<UpdateHistoryItem>()
            .FirstOrDefault(e => e.CultureName == cultureInfo.Name);
    }

    public bool UpdateItem(UpdateHistoryItem updateItem)
    {
        var rows = databaseConnection.Update(updateItem);

        return rows == 1;
    }

    #endregion Interface Implementations
}


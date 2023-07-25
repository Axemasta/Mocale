using Mocale.DAL.Entities;

namespace Mocale.DAL.Repositories;

public class CacheRepository : ICacheRepository
{
    private readonly SQLiteConnection databaseConnection;

    public CacheRepository(IDatabaseConnectionProvider databaseConnectionProvider)
    {
        this.databaseConnection = databaseConnectionProvider.GetDatabaseConnection();

        databaseConnection.CreateTable<UpdateItem>();
    }
}


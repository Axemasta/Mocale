using Microsoft.Extensions.Logging;
namespace Mocale.Cache.SQLite.Repositories;

public abstract class RepositoryBase
{
    protected ILogger logger { get; }

    protected SQLiteConnection Connection { get; }

    protected RepositoryBase(
        IDatabaseConnectionProvider databaseConnectionProvider,
        ILogger logger)
    {
        databaseConnectionProvider = Guard.Against.Null(databaseConnectionProvider, nameof(databaseConnectionProvider));
        this.logger = Guard.Against.Null(logger, nameof(logger));

        Connection = databaseConnectionProvider.GetDatabaseConnection();
    }
}

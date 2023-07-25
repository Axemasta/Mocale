using Microsoft.Extensions.Logging;
namespace Mocale.Cache.SQLite.Providers;

public class DatabaseConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IDatabasePathProvider databasePathProvider;
    private readonly ILogger logger;

    public DatabaseConnectionProvider(
        IDatabasePathProvider databasePathProvider,
        ILogger<DatabaseConnectionProvider> logger)
    {
        this.databasePathProvider = Guard.Against.Null(databasePathProvider, nameof(databasePathProvider));
        this.logger = Guard.Against.Null(logger, nameof(logger));
    }

    public SQLiteConnection GetDatabaseConnection()
    {
        var databasePath = databasePathProvider.GetDatabasePath();

        logger.LogTrace("Opening connecting to database: {DatabasePath}", databasePath);

        return new SQLiteConnection(
            databasePath,
            SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite,
            true);
    }
}

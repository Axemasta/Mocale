using Microsoft.Extensions.Logging;
namespace Mocale.Cache.SQLite.Providers;

internal class DatabaseConnectionProvider : IDatabaseConnectionProvider
{
    #region Fields

    private readonly Lazy<SQLiteConnection> connectionLazy;
    private readonly IDatabasePathProvider databasePathProvider;
    private readonly ILogger logger;

    #endregion

    #region Constructors

    public DatabaseConnectionProvider(
        IDatabasePathProvider databasePathProvider,
        ILogger<DatabaseConnectionProvider> logger)
    {
        this.databasePathProvider = Guard.Against.Null(databasePathProvider, nameof(databasePathProvider));
        this.logger = Guard.Against.Null(logger, nameof(logger));

        // This could cause issues if we need to rebuild the connection...
        this.connectionLazy = new Lazy<SQLiteConnection>(BuildConnection, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    #endregion Constructors

    #region Methods

    private SQLiteConnection BuildConnection()
    {
        var databasePath = databasePathProvider.GetDatabasePath();

        logger.LogTrace("Opening connecting to database: {DatabasePath}", databasePath);

        return new SQLiteConnection(
            databasePath,
            SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite,
            true);
    }

    #endregion Methods

    #region Interface Implementations

    public SQLiteConnection GetDatabaseConnection()
    {
        return connectionLazy.Value;
    }

    #endregion Interface Implementations
}

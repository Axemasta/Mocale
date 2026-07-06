using Microsoft.Extensions.Logging;

namespace Mocale.Cache.SQLite.Providers;

internal partial class DatabaseConnectionProvider : IDatabaseConnectionProvider
{
    #region Fields

    private readonly Lazy<SQLiteConnection> connectionLazy;
    private readonly IDatabasePathProvider databasePathProvider;
    private readonly ILogger logger;

    #endregion Fields

    #region Constructors

    public DatabaseConnectionProvider(
        IDatabasePathProvider databasePathProvider,
        ILogger<DatabaseConnectionProvider> logger)
    {
        this.databasePathProvider = Guard.Against.Null(databasePathProvider);
        this.logger = Guard.Against.Null(logger);

        // This could cause issues if we need to rebuild the connection...
        connectionLazy = new Lazy<SQLiteConnection>(BuildConnection, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    #endregion Constructors

    #region Methods

    private SQLiteConnection BuildConnection()
    {
        var databasePath = databasePathProvider.GetDatabasePath();

        LogOpeningConnectingToDatabaseDatabasePath(databasePath);

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

    #region Logging

    [LoggerMessage(LogLevel.Trace, "Opening connecting to database: {DatabasePath}")]
    partial void LogOpeningConnectingToDatabaseDatabasePath(string databasePath);

    #endregion Logging
}

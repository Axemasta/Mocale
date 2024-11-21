using Microsoft.Extensions.Logging;
namespace Mocale.Cache.SQLite.Repositories;

internal abstract class RepositoryBase
{
#pragma warning disable IDE1006
    // ReSharper disable once InconsistentNaming
    protected ILogger logger { get; }
#pragma warning restore IDE1006

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

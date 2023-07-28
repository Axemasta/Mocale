namespace Mocale.Cache.SQLite.Abstractions;

public interface IDatabaseConnectionProvider
{
    SQLiteConnection GetDatabaseConnection();
}

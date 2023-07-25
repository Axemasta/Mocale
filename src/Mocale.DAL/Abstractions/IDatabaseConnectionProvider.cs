namespace Mocale.DAL.Abstractions;

public interface IDatabaseConnectionProvider
{
    SQLiteConnection GetDatabaseConnection();
}


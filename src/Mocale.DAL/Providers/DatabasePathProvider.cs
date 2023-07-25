namespace Mocale.DAL.Providers;

public class DatabasePathProvider : IDatabasePathProvider
{
    private readonly IFileSystem fileSystem;

    public DatabasePathProvider(IFileSystem fileSystem)
    {
        this.fileSystem = Guard.Against.Null(fileSystem, nameof(fileSystem));
    }

    public string GetDatabasePath()
    {
        return Path.Combine(fileSystem.AppDataDirectory, Constants.DatabaseFileName);
    }
}


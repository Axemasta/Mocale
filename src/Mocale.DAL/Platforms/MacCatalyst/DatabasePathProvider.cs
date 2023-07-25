using Microsoft.Extensions.Logging;

namespace Mocale.DAL.Platforms.MacCatalyst;

public class DatabasePathProvider : IDatabasePathProvider
{
    private readonly IFileSystem fileSystem;
    private readonly ILogger logger;

    public DatabasePathProvider(
        IFileSystem fileSystem,
        ILogger<DatabasePathProvider> logger)
    {
        this.fileSystem = Guard.Against.Null(fileSystem, nameof(fileSystem));
        this.logger = Guard.Against.Null(logger, nameof(logger));
    }

    public string GetDatabasePath()
    {
        /*
         * Maui's filesystem implementation is incredibly awkward on mac catalyst since it uses ~/Library
         * instead of an app sandbox.
         * 
         * Here we are going to create a directory in this location to use as an app sandbox.
         * 
         * The path created will be:
         * ~/Library/Application Support/com.acme.yoursapp/
         */

        return Path.Combine(fileSystem.AppDataDirectory, Constants.DatabaseFileName);

        var directory = Path.Combine(fileSystem.AppDataDirectory, "Application Support", AppInfo.PackageName);

        if (!CreateIfNotExists(directory))
        {
            logger.LogWarning("Directory check failed, this may result in unexpected behaviour");
        }

        return Path.Combine(directory, Constants.DatabaseFileName);
    }

    private bool CreateIfNotExists(string directory)
    {
        // https://github.com/dotnet/maui/issues/7657#issuecomment-1145545635

        try
        {
            if (!Directory.Exists(directory))
            {
                var info = Directory.CreateDirectory(directory);

                logger.LogDebug("Created mocale cache directory: {Path}", info.FullName);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred checking for / creating driectory: {Path}", directory);

            return false;
        }
    }
}

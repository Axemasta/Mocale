using System.Globalization;
using System.Reflection;
using System.Text.Json;
namespace Mocale.Providers;

internal class EmbeddedResourceProvider : IInternalLocalizationProvider
{
    private readonly IEmbeddedResourcesConfig localConfig;
    private readonly ILogger logger;

    public EmbeddedResourceProvider(
        IConfigurationManager<IEmbeddedResourcesConfig> jsonConfigurationManager,
        ILogger<EmbeddedResourceProvider> logger)
    {
        localConfig = jsonConfigurationManager.Configuration;
        this.logger = logger;
    }

    public Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo)
    {
        // read assembly
        if (localConfig.ResourcesAssembly is null)
        {
            logger.LogWarning("Configured resource assembly was null");
            return new Dictionary<string, string>();
        }

        var resources = localConfig.ResourcesAssembly.GetManifestResourceNames();

        if (resources is null)
        {
            logger.LogWarning("No embedded resources found in assembly {ResourceAssembly}", localConfig.ResourcesAssembly);
            return new Dictionary<string, string>();
        }

        // look for the right folder
        var relativeFolder = localConfig.UseResourceFolder
            ? $"Resources.{localConfig.ResourcesPath}"
            : localConfig.ResourcesPath;

        var folderPrefix = localConfig.ResourcesAssembly.GetName().Name + "." + relativeFolder;

        var localesFolderResources = resources.Where(r => r.StartsWith(folderPrefix, StringComparison.InvariantCultureIgnoreCase));

        if (!localesFolderResources.Any())
        {
            logger.LogWarning("No assembly resources found with prefix: {FolderPrefix}", folderPrefix);
            return new Dictionary<string, string>();
        }

        // check if filenames match
        var cultureMatch = localesFolderResources.FirstOrDefault(r => FileMatchesCulture(r, cultureInfo));

        if (cultureMatch != null)
        {
            // deserialize if match
            return ParseFile(cultureMatch, localConfig.ResourcesAssembly);
        }

        logger.LogWarning("Unable to find resource for selected culture: {CultureName}", cultureInfo.Name);

        return new Dictionary<string, string>();
    }

    private static bool FileMatchesCulture(string resourceName, CultureInfo culture)
    {
        // Cracking coding here 🍝
        var resourcePath = resourceName.Replace('.', '/');
        resourcePath = resourcePath.Replace("/json", ".json");

        var fileName = Path.GetFileNameWithoutExtension(resourcePath);

        return fileName.Equals(culture.Name, StringComparison.OrdinalIgnoreCase);
    }

    private Dictionary<string, string> ParseFile(string filePath, Assembly assembly)
    {
        using var fileStream = assembly.GetManifestResourceStream(filePath);

        if (fileStream is null)
        {
            logger.LogWarning("File stream was null for assembly resource: {FilePath}", filePath);
            return new Dictionary<string, string>();
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(fileStream)
                ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred loading & parsing assembly resource {FilePath}", filePath);

            return new Dictionary<string, string>();
        }
    }
}

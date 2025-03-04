using System.Globalization;
using System.Reflection;
using System.Text.Json;
namespace Mocale.Providers;

internal class EmbeddedResourceProvider(
    IConfigurationManager<IEmbeddedResourcesConfig> jsonConfigurationManager,
    ILogger<EmbeddedResourceProvider> logger)
    : IInternalLocalizationProvider
{
    private readonly IEmbeddedResourcesConfig localConfig = jsonConfigurationManager.Configuration;
    private readonly ILogger logger = logger;

    public Dictionary<string, string>? GetValuesForCulture(CultureInfo cultureInfo)
    {
        // read assembly
        if (localConfig.ResourcesAssembly is null)
        {
            logger.LogWarning("Configured resource assembly was null");
            return null;
        }

        var resources = localConfig.ResourcesAssembly.GetManifestResourceNames();

        // look for the right folder
        var relativeFolder = localConfig.UseResourceFolder
            ? $"Resources.{localConfig.ResourcesPath}"
            : localConfig.ResourcesPath;

        var folderPrefix = localConfig.ResourcesAssembly.GetName().Name + "." + relativeFolder;

        var localesFolderResources = resources.Where(r => r.StartsWith(folderPrefix, StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        if (localesFolderResources.Count < 1)
        {
            logger.LogWarning("No assembly resources found with prefix: {FolderPrefix}", folderPrefix);
            return null;
        }

        // check if filenames match
        var cultureMatch = localesFolderResources.FirstOrDefault(r => FileMatchesCulture(r, cultureInfo));

        if (cultureMatch != null)
        {
            // deserialize if match
            return ParseFile(cultureMatch, localConfig.ResourcesAssembly);
        }

        logger.LogWarning("Unable to find resource for selected culture: {CultureName}", cultureInfo.Name);

        return null;
    }

    private static bool FileMatchesCulture(string resourceName, CultureInfo culture)
    {
        // Cracking coding here 🍝
        var resourcePath = resourceName.Replace('.', '/');
        resourcePath = resourcePath.Replace("/json", ".json");

        var fileName = Path.GetFileNameWithoutExtension(resourcePath);

        return fileName.Equals(culture.Name, StringComparison.OrdinalIgnoreCase);
    }

    internal Dictionary<string, string>? ParseFile(string filePath, Assembly assembly)
    {
        using var fileStream = assembly.GetManifestResourceStream(filePath);

        if (fileStream is null)
        {
            logger.LogWarning("File stream was null for assembly resource: {FilePath}", filePath);
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(fileStream) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred loading & parsing assembly resource {FilePath}", filePath);

            return null;
        }
    }
}

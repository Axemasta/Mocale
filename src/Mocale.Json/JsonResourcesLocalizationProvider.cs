using System;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Json.Abstractions;

namespace Mocale.Json;

internal class JsonResourcesLocalizationProvider : ILocalizationProvider
{
    private readonly IMocaleConfiguration globalConfiguration;
    private readonly IJsonResourcesConfig localConfig;
    private readonly ILogger logger;

    public JsonResourcesLocalizationProvider(
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        IConfigurationManager<IJsonResourcesConfig> jsonConfigurationManager,
        ILogger<JsonResourcesLocalizationProvider> logger)
    {
        this.globalConfiguration = mocaleConfigurationManager.GetConfiguration();
        this.localConfig = jsonConfigurationManager.GetConfiguration();
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
            logger.LogWarning("No embedded resources found in assembly {0}", localConfig.ResourcesAssembly);
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
            logger.LogWarning("No assembly resources found with prefix: {0}", folderPrefix);
            return new Dictionary<string, string>();
        }

        // check if filenames match
        var cultureMatch = localesFolderResources.FirstOrDefault(r => FileMatchesCulture(r, cultureInfo));

        if (cultureMatch != null)
        {
            // deserialize if match
            return ParseFile(cultureMatch, localConfig.ResourcesAssembly);
        }

        logger.LogWarning("Unable to find resource for selected culture: {0}", cultureInfo.Name);

        return new Dictionary<string, string>();
    }

    private bool FileMatchesCulture(string resourceName, CultureInfo culture)
    {
        // Cracking coding here 🍝
        var resourcePath = resourceName.Replace('.', '/');
        resourcePath = resourcePath.Replace("/json", ".json");

        var fileName = Path.GetFileNameWithoutExtension(resourcePath);

        return fileName.Equals(culture.Name, StringComparison.OrdinalIgnoreCase);
    }

    private Dictionary<string, string> ParseFile(string filePath, Assembly assembly)
    {
        using (var fileStream = assembly.GetManifestResourceStream(filePath))
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(fileStream);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred loading & parsing assembly resource {0}", filePath);

                return new Dictionary<string, string>();
            }
        }
    }
}

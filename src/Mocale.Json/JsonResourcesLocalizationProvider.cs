using System;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Mocale.Abstractions;
using Mocale.Json.Abstractions;

namespace Mocale.Json
{
    internal class JsonResourcesLocalizationProvider : ILocalizationProvider
    {
        private readonly IMocaleConfiguration globalConfiguration;
        private readonly IJsonResourcesConfig localConfig;

        public JsonResourcesLocalizationProvider(
            IMocaleConfiguration mocaleConfiguration,
            IJsonResourcesConfig jsonResourcesConfig)
        {
            this.globalConfiguration = mocaleConfiguration;
            this.localConfig = jsonResourcesConfig;
        }

        public Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo)
        {
            // read assembly
            if (localConfig.ResourcesAssembly is null)
            {
                Console.WriteLine("No Resources Assembly provided");
                return new Dictionary<string, string>();
            }

            var resources = localConfig.ResourcesAssembly.GetManifestResourceNames();

            if (resources is null)
            {
                Console.WriteLine($"No resources found in assembly: {localConfig.ResourcesAssembly}");
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
                Console.WriteLine($"No resources found with prefix: {folderPrefix}");
                return new Dictionary<string, string>();
            }

            // check if filenames match
            var cultureMatch = localesFolderResources.FirstOrDefault(r => FileMatchesCulture(r, cultureInfo));

            if (cultureMatch != null)
            {
                // deserialize if match
                return ParseFile(cultureMatch, localConfig.ResourcesAssembly);
            }

            // Lookup default
            var defaultMatch = localesFolderResources.FirstOrDefault(r => FileMatchesCulture(r, globalConfiguration.DefaultCulture));

            if (defaultMatch != null)
            {
                // deserialize if match
                return ParseFile(defaultMatch, localConfig.ResourcesAssembly);
            }

            Console.WriteLine($"Unable to find resource for selected culture: {cultureInfo.Name}, or default culture: {globalConfiguration.DefaultCulture.Name}");
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
                    Console.WriteLine($"Unable to parse file as dictionary: {filePath}");
                    Console.WriteLine(ex);
                    return new Dictionary<string, string>();
                }
            }
        }
    }
}


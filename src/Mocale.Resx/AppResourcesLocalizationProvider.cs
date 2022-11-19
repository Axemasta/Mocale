using System.Collections;
using System.Globalization;
using System.Resources;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Resx.Abstractions;

namespace Mocale.Resx;

internal class AppResourcesLocalizationProvider : ILocalizationProvider
{
    private readonly ResourceManager resourceManager;
    private readonly ILogger logger;
    private readonly IAppResourcesConfig appResourcesConfig;

    public AppResourcesLocalizationProvider(
        IConfigurationManager<IAppResourcesConfig> appResourcesConfigurationManager,
        ILogger<AppResourcesLocalizationProvider> logger)
    {
        this.appResourcesConfig = appResourcesConfigurationManager.GetConfiguration();
        this.resourceManager = new ResourceManager(appResourcesConfig.AppResourcesType);
        this.logger = logger;
    }

    public Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo)
    {
        // https://stackoverflow.com/a/1970941/8828057
        var resourceSet = resourceManager.GetResourceSet(cultureInfo, true, false);

        if (resourceSet is null)
        {
            // Currently if the AppResources represents en-GB this path will be taken.
            // This is because en-GB is the default and doesnt have the explicit cultural naming
            // So GetResourceSet will return null and this path will be followed.
            //
            // If the default value isn't equal to the app resources default I dont even know
            // if there is a real way of knowing what culture the app.resources is?
            // Maybe this is one of those semi bugs that wont be fixed :D

            logger.LogWarning("Unable to load resources for culture: {0}, reverting to default", cultureInfo.Name);

            var defaultSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);

            return defaultSet.Cast<DictionaryEntry>()
            .ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
        }

        return resourceSet.Cast<DictionaryEntry>()
            .ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
    }
}

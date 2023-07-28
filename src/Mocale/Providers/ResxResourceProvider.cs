using System.Collections;
using System.Globalization;
using System.Resources;
using Mocale.Exceptions;

namespace Mocale.Providers;

internal class AppResourceProvider : IInternalLocalizationProvider
{
    private readonly ResourceManager resourceManager;
    private readonly IAppResourcesConfig appResourcesConfig;
    private readonly IMocaleConfiguration mocaleConfiguration;

    public AppResourceProvider(
        IConfigurationManager<IAppResourcesConfig> appResourcesConfigurationManager,
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager)
    {
        appResourcesConfig = appResourcesConfigurationManager.Configuration;
        mocaleConfiguration = mocaleConfigurationManager.Configuration;

        if (appResourcesConfig.AppResourcesType is null)
        {
            throw new InitializationException("App Resource Type has not been set, this should be configured during startup");
        }

        resourceManager = new ResourceManager(appResourcesConfig.AppResourcesType);
    }

    private static Dictionary<string, string> ConvertResourceSet(ResourceSet resourceSet)
    {
        // This cast wants to fight me and I can't figure out a nullable friendly way of doing this.
        // Instead of wasting time on syntax, leave it here and figure it out later!
#nullable disable
        return resourceSet.Cast<DictionaryEntry>().ToDictionary(r => r.Key.ToString(), r => r.Value.ToString())
            ?? new Dictionary<string, string>();
#nullable enable
    }

    public Dictionary<string, string>? GetValuesForCulture(CultureInfo cultureInfo)
    {
        // https://stackoverflow.com/a/1970941/8828057
        var resourceSet = resourceManager.GetResourceSet(cultureInfo, true, false);

        if (resourceSet is null)
        {
            // The default culture will not have a name (ie en-GB.resx) so we will have to trust the
            // user configured this correctly?!
            if (cultureInfo.Equals(mocaleConfiguration.DefaultCulture))
            {
                var defaultSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);

                if (defaultSet is null)
                {
                    return null;
                }

                return ConvertResourceSet(defaultSet);
            }

            // Since we don't know about this culture, lets not return a default

            return null;
        }

        return ConvertResourceSet(resourceSet);
    }
}

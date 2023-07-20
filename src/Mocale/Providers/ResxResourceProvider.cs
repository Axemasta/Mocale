using System.Collections;
using System.Globalization;
using System.Resources;
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
        appResourcesConfig = appResourcesConfigurationManager.GetConfiguration();
        mocaleConfiguration = mocaleConfigurationManager.GetConfiguration();

        resourceManager = new ResourceManager(appResourcesConfig.AppResourcesType);
    }

    public Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo)
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

                return defaultSet.Cast<DictionaryEntry>()
                    .ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
            }

            // Since we don't know about this culture, lets not return a default

            return null;
        }

        return resourceSet.Cast<DictionaryEntry>()
            .ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
    }
}

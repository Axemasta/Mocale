using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;
using Mocale.Resx.Abstractions;
using Mocale.Resx.Models;

namespace Mocale.Resx;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseAppResources(this MocaleBuilder builder, Action<AppResourcesConfig> configureResources)
    {
        builder.ConfigurationManager.UpdateConfiguration(config => ((MocaleConfiguration)config).ResourceType = LocalResourceType.Resx);

        var config = new AppResourcesConfig();
        configureResources.Invoke(config);

        var appResourcesConfigManager = new ConfigurationManager<IAppResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IAppResourcesConfig>>(appResourcesConfigManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, AppResourcesLocalizationProvider>();

        return builder;
    }
}

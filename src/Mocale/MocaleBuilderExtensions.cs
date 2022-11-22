using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Managers;
using Mocale.Models;
using Mocale.Providers;

namespace Mocale;

public static class MocaleBuilderExtensions
{
    public static MocaleBuilder UseAppResources(this MocaleBuilder builder, Action<AppResourcesConfig> configureResources)
    {
        builder.ConfigurationManager.UpdateConfiguration(config => ((MocaleConfiguration)config).ResourceType = LocaleResourceType.Resx);

        var config = new AppResourcesConfig();
        configureResources.Invoke(config);

        var appResourcesConfigManager = new ConfigurationManager<IAppResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IAppResourcesConfig>>(appResourcesConfigManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, AppResourceProvider>();

        return builder;
    }

    public static MocaleBuilder UseEmbeddedResources(this MocaleBuilder builder, Action<EmbeddedResourcesConfig> configureJson)
    {
        builder.ConfigurationManager.UpdateConfiguration(config => ((MocaleConfiguration)config).ResourceType = LocaleResourceType.Json);

        var config = new EmbeddedResourcesConfig();
        configureJson.Invoke(config);

        var embeddedResourcesConfigManager = new ConfigurationManager<IEmbeddedResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IEmbeddedResourcesConfig>>(embeddedResourcesConfigManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, EmbeddedResourceProvider>();

        return builder;
    }
}

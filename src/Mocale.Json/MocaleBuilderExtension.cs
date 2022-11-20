using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Json.Abstractions;
using Mocale.Json.Models;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale.Json;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseJsonResources(this MocaleBuilder builder, Action<JsonResourcesConfig> configureJson)
    {
        builder.ConfigurationManager.UpdateConfiguration(config => ((MocaleConfiguration)config).ResourceType = LocalResourceType.Json);

        var config = new JsonResourcesConfig();
        configureJson.Invoke(config);

        var jsonConfigurationManager = new ConfigurationManager<IJsonResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IJsonResourcesConfig>>(jsonConfigurationManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, JsonResourcesLocalizationProvider>();

        return builder;
    }
}

using Mocale.Abstractions;
using Mocale.Json.Abstractions;
using Mocale.Json.Models;
using Mocale.Managers;

namespace Mocale.Json;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder WithJsonResourcesProvider(this MocaleBuilder builder, Action<JsonResourcesConfig> resourceConfig)
    {
        var config = new JsonResourcesConfig();
        resourceConfig.Invoke(config);

        var jsonConfigurationManager = new ConfigurationManager<IJsonResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IJsonResourcesConfig>>(jsonConfigurationManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, JsonResourcesLocalizationProvider>();

        return builder;
    }
}

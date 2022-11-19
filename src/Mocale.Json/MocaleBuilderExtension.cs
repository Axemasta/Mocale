using System;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Json.Abstractions;
using Mocale.Json.Managers;
using Mocale.Json.Models;
using Mocale.Services;

namespace Mocale.Json
{
    public static class MocaleBuilderExtension
    {
        public static MocaleBuilder WithJsonResourcesProvider(this MocaleBuilder builder, Action<JsonResourcesConfig> resourceConfig)
        {
            var config = new JsonResourcesConfig();
            resourceConfig.Invoke(config);

            var jsonConfigurationManager = new JsonConfigurationManager();
            jsonConfigurationManager.SetConfiguration(config);

            builder.AppBuilder.Services.AddSingleton<IJsonConfigurationManager>(jsonConfigurationManager);
            builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, JsonResourcesLocalizationProvider>();

            return builder.WithLocalizationProvider(() => AppBuilderExtensions.ServiceProvider.GetRequiredService<ILocalizationProvider>());
        }
    }
}

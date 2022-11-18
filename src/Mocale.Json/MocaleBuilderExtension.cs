using System;
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

            var globalConfig = ConfigurationManager.Instance.GetConfiguration();

            var provider = new JsonResourcesLocalizationProvider(config);

            return builder.WithLocalizationProvider(() => provider);
        }
    }
}


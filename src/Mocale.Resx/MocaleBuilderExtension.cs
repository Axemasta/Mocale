using Mocale.Abstractions;
using Mocale.Managers;
using Mocale.Resx.Abstractions;
using Mocale.Resx.Models;

namespace Mocale.Resx;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder WithAppResourcesProvider(this MocaleBuilder builder, Action<AppResourcesConfig> resourceConfig)
    {
        var config = new AppResourcesConfig();
        resourceConfig.Invoke(config);

        var appResourcesConfigManager = new ConfigurationManager<IAppResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IAppResourcesConfig>>(appResourcesConfigManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, AppResourcesLocalizationProvider>();

        return builder;
    }
}

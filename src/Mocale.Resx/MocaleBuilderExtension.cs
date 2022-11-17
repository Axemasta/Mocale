using Mocale.Resx.Models;
using Mocale.Services;

namespace Mocale.Resx;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder WithAppResourcesProvider(this MocaleBuilder builder, Action<AppResourcesConfig> resourceConfig)
    {
        var config = new AppResourcesConfig();
        resourceConfig.Invoke(config);

        var globalConfig = ConfigurationManager.Instance.GetConfiguration();

        var provider = new AppResourcesLocalizationProvider(config);

        return builder.WithLocalizationProvider(() => provider);
    }
}

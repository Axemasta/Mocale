using Microsoft.Extensions.Logging;
using Mocale.Abstractions;
using Mocale.Resx.Models;
using Mocale.Services;

namespace Mocale.Resx;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder WithAppResourcesProvider(this MocaleBuilder builder, Action<AppResourcesConfig> resourceConfig)
    {
        var config = new AppResourcesConfig();
        resourceConfig.Invoke(config);

        builder.AppBuilder.Services.AddSingleton<ILocalizationProvider, AppResourcesLocalizationProvider>();

        return builder.WithLocalizationProvider(() => AppBuilderExtensions.ServiceProvider.GetRequiredService<ILocalizationProvider>());
    }
}
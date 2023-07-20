using Mocale.Exceptions;
using Mocale.Managers;
using Mocale.Providers;
namespace Mocale;

public static class MocaleBuilderExtensions
{
    public static MocaleBuilder UseAppResources(this MocaleBuilder builder, Action<AppResourcesConfig> configureResources)
    {
        builder.RegisterLocalProvider(typeof(AppResourceProvider));

        builder.ConfigurationManager.UpdateConfiguration(config => ((MocaleConfiguration)config).ResourceType = LocaleResourceType.Resx);

        var config = new AppResourcesConfig();
        configureResources.Invoke(config);

        var appResourcesConfigManager = new ConfigurationManager<IAppResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IAppResourcesConfig>>(appResourcesConfigManager);
        builder.AppBuilder.Services.AddSingleton<IInternalLocalizationProvider, AppResourceProvider>();

        return builder;
    }

    public static MocaleBuilder UseEmbeddedResources(this MocaleBuilder builder, Action<EmbeddedResourcesConfig> configureJson)
    {
        builder.RegisterLocalProvider(typeof(EmbeddedResourceProvider));

        builder.ConfigurationManager.UpdateConfiguration(config => ((MocaleConfiguration)config).ResourceType = LocaleResourceType.Json);

        var config = new EmbeddedResourcesConfig();
        configureJson.Invoke(config);

        var embeddedResourcesConfigManager = new ConfigurationManager<IEmbeddedResourcesConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IEmbeddedResourcesConfig>>(embeddedResourcesConfigManager);
        builder.AppBuilder.Services.AddSingleton<IInternalLocalizationProvider, EmbeddedResourceProvider>();

        return builder;
    }

    internal static void RegisterLocalProvider(this MocaleBuilder builder, Type provider)
    {
        if (!provider.IsAssignableTo(typeof(IInternalLocalizationProvider)))
        {
            throw new InitializationException($"The provider is not of type {nameof(IInternalLocalizationProvider)}");
        }

        if (builder.LocalProviderRegistered)
        {
            throw new InitializationException($"The following local provider was already registered: {builder.LocalProviderName}");
        }

        builder.LocalProviderRegistered = true;
        builder.LocalProviderName = provider.Name;
    }

    internal static void RegisterExternalProvider(this MocaleBuilder builder, Type provider)
    {
        if (!provider.IsAssignableTo(typeof(IExternalLocalizationProvider)))
        {
            throw new InitializationException($"The provider is not of type {nameof(IExternalLocalizationProvider)}");
        }

        if (builder.ExternalProviderRegistered)
        {
            throw new InitializationException($"The following local provider was already registered: {builder.ExternalProviderName}");
        }

        builder.ExternalProviderRegistered = true;
        builder.ExternalProviderName = provider.Name;
    }
}

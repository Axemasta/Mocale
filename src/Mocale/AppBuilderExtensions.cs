using Mocale.Abstractions;
using Mocale.Exceptions;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale;

/// <summary>
/// Host build extensions for Mocale
/// </summary>
public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMocale(
        this MauiAppBuilder mauiAppBuilder,
        Action<MocaleBuilder> builder = default)
    {
        // This prevents us from having a static service locator
        var appDescriptor = mauiAppBuilder.Services.FirstOrDefault(s => s.ServiceType == typeof(IApplication));

        if (!appDescriptor.ImplementationType.IsAssignableTo(typeof(IAppServiceProvider)))
        {
            throw new InitializationException($"Your MauiApp must implement {nameof(IAppServiceProvider)}, please review setup documentation for further instruction");
        }

        var mocaleBuilder = new MocaleBuilder()
        {
            AppBuilder = mauiAppBuilder, // Give the builders a reference so they can register things
        };

        // Invoke mocaleConfiguration action
        builder?.Invoke(mocaleBuilder);

        // Default config if the consumer doesn't call WithConfiguration(...)
        mocaleBuilder.ConfigurationManager ??= new ConfigurationManager<IMocaleConfiguration>(new MocaleConfiguration());

        mauiAppBuilder.Services.AddSingleton<IConfigurationManager<IMocaleConfiguration>>(mocaleBuilder.ConfigurationManager);
        mauiAppBuilder.Services.AddSingleton<ILocalizationManager, LocalizationManager>();

        if (!mocaleBuilder.LocalProviderRegistered)
        {
            throw new InitializationException($"No local provider has been registered, please call either {nameof(MocaleBuilderExtensions.UseAppResources)} or {nameof(MocaleBuilderExtensions.UseEmbeddedResources)} in order to use mocale");
        }

        var config = mocaleBuilder.ConfigurationManager.GetConfiguration();

        if (config.UseExternalProvider && !mocaleBuilder.ExternalProviderRegistered)
        {
            throw new InitializationException($"No external provider was registered when mocale was configured to use one. Please register an external provider or set {nameof(IMocaleConfiguration.UseExternalProvider)} to false.");
        }

        return mauiAppBuilder;
    }
}

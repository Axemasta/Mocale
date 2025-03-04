using Microsoft.Extensions.Caching.Memory;
using Mocale.Cache;
using Mocale.Exceptions;
using Mocale.Managers;
using Mocale.Providers;
namespace Mocale;

/// <summary>
/// Host build extensions for Mocale
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    /// Use mocale in your app to provide translations.
    /// </summary>
    /// <param name="mauiAppBuilder">Maui app builder</param>
    /// <param name="builder">Mocale builder</param>
    /// <returns>Maui app builder</returns>
    /// <exception cref="InitializationException">If configuration is invalid, see details for troubleshooting</exception>
    public static MauiAppBuilder UseMocale(
        this MauiAppBuilder mauiAppBuilder,
        Action<MocaleBuilder>? builder = default)
    {
        var mocaleBuilder = new MocaleBuilder
        {
            AppBuilder = mauiAppBuilder, // Give the builders a reference so they can register things,
            ConfigurationManager = new ConfigurationManager<IMocaleConfiguration>(new MocaleConfiguration()),
        };

        // Invoke mocaleConfiguration action
        builder?.Invoke(mocaleBuilder);

        // Register DI Services
        // - Maui Dependencies
        mauiAppBuilder.Services.AddSingleton(Preferences.Default);
        mauiAppBuilder.Services.AddSingleton(FileSystem.Current);
        mauiAppBuilder.Services.AddSingleton(TimeProvider.System);

        // - Localization
        mauiAppBuilder.Services.AddSingleton<IConfigurationManager<IMocaleConfiguration>>(mocaleBuilder.ConfigurationManager);
        mauiAppBuilder.Services.AddSingleton<ILocalizationManager, LocalizationManager>();
        mauiAppBuilder.Services.AddSingleton<IMauiInitializeService, MocaleInitializeService>();
        mauiAppBuilder.Services.AddSingleton<ITranslationResolver, TranslationResolver>();
        mauiAppBuilder.Services.AddSingleton<ICurrentCultureManager, CurrentCultureManager>();
        mauiAppBuilder.Services.AddSingleton<IInternalTranslatorManager, TranslatorManager>();

        // https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
        mauiAppBuilder.Services.AddSingleton<TranslatorManager>();
        mauiAppBuilder.Services.AddSingleton<ITranslatorManager>(x => x.GetRequiredService<TranslatorManager>());
        mauiAppBuilder.Services.AddSingleton<ITranslationUpdater>(x => x.GetRequiredService<TranslatorManager>());
        mauiAppBuilder.Services.AddSingleton<IInternalTranslatorManager>(x => x.GetRequiredService<TranslatorManager>());

        if (!mocaleBuilder.LocalProviderRegistered)
        {
            throw new InitializationException($"No local provider has been registered, please call either {nameof(MocaleBuilderExtensions.UseAppResources)} or {nameof(MocaleBuilderExtensions.UseEmbeddedResources)} in order to use mocale");
        }

        var config = mocaleBuilder.ConfigurationManager.Configuration;

        if (config.UseExternalProvider && !mocaleBuilder.ExternalProviderRegistered)
        {
            throw new InitializationException($"No external provider was registered when mocale was configured to use one. Please register an external provider or set {nameof(IMocaleConfiguration.UseExternalProvider)} to false.");
        }

        if (!config.UseExternalProvider)
        {
            mauiAppBuilder.Services.AddTransient<IExternalLocalizationProvider, InactiveExternalLocalizationProvider>();
        }

        if (!mocaleBuilder.CacheProviderRegistered)
        {
            mauiAppBuilder.Services.AddTransient<MemoryCache>();
            mauiAppBuilder.Services.AddSingleton<InMemoryCacheManager>();
            mauiAppBuilder.Services.AddSingleton<ILocalisationCacheManager>(x => x.GetRequiredService<InMemoryCacheManager>());
            mauiAppBuilder.Services.AddSingleton<ICacheUpdateManager>(x => x.GetRequiredService<InMemoryCacheManager>());
        }

        return mauiAppBuilder;
    }
}

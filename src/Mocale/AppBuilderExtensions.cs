using Mocale.Exceptions;
using Mocale.Helper;
using Mocale.Managers;
using Mocale.Wrappers;
namespace Mocale;

/// <summary>
/// Host build extensions for Mocale
/// </summary>
public static class AppBuilderExtensions
{
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
        mauiAppBuilder.Services.AddTransient<IDateTime, DateTimeWrapper>();

        // - Localization
        mauiAppBuilder.Services.AddSingleton<IConfigurationManager<IMocaleConfiguration>>(mocaleBuilder.ConfigurationManager);
        mauiAppBuilder.Services.AddSingleton<ILocalizationManager, LocalizationManager>();
        mauiAppBuilder.Services.AddSingleton<IMauiInitializeService, MocaleInitializeService>();
        mauiAppBuilder.Services.AddSingleton<ITranslationResolver, TranslationResolver>();
        mauiAppBuilder.Services.AddSingleton<ICurrentCultureManager, CurrentCultureManager>();
        mauiAppBuilder.Services.AddSingleton<IVersionPrefixHelper, VersionPrefixHelper>();

        // https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
        mauiAppBuilder.Services.AddSingleton<TranslatorManager>();
        mauiAppBuilder.Services.AddSingleton<ITranslatorManager>(x => x.GetRequiredService<TranslatorManager>());
        mauiAppBuilder.Services.AddSingleton<ITranslationUpdater>(x => x.GetRequiredService<TranslatorManager>());

        if (!mocaleBuilder.LocalProviderRegistered)
        {
            throw new InitializationException($"No local provider has been registered, please call either {nameof(MocaleBuilderExtensions.UseAppResources)} or {nameof(MocaleBuilderExtensions.UseEmbeddedResources)} in order to use mocale");
        }

        var config = mocaleBuilder.ConfigurationManager.Configuration;

        if (config.UseExternalProvider && !mocaleBuilder.ExternalProviderRegistered)
        {
            throw new InitializationException($"No external provider was registered when mocale was configured to use one. Please register an external provider or set {nameof(IMocaleConfiguration.UseExternalProvider)} to false.");
        }

        if (!mocaleBuilder.CacheProviderRegistered)
        {
            // TODO: Initialize in memory provider
            throw new InitializationException("No cache provider has been registered. In the future there will be an in memory provider, for now please install and register the SQLite cache provider");
        }

        return mauiAppBuilder;
    }
}

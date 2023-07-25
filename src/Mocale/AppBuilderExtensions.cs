using Mocale.DAL;
using Mocale.DAL.Abstractions;
using Mocale.DAL.Providers;
using Mocale.DAL.Repositories;
using Mocale.Exceptions;
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

        // - Caching
        mauiAppBuilder.Services.AddTransient<ICacheUpdateManager, CacheUpdateManager>();
        mauiAppBuilder.Services.AddSingleton<IDatabaseConnectionProvider, DatabaseConnectionProvider>();
        mauiAppBuilder.Services.AddTransient<IDatabasePathProvider, DatabasePathProvider>();

        mauiAppBuilder.Services.AddSingleton<ICacheRepository, CacheRepository>();

        if (!mocaleBuilder.LocalProviderRegistered)
        {
            throw new InitializationException($"No local provider has been registered, please call either {nameof(MocaleBuilderExtensions.UseAppResources)} or {nameof(MocaleBuilderExtensions.UseEmbeddedResources)} in order to use mocale");
        }

        var config = mocaleBuilder.ConfigurationManager.Configuration;

        if (config.UseExternalProvider && !mocaleBuilder.ExternalProviderRegistered)
        {
            throw new InitializationException($"No external provider was registered when mocale was configured to use one. Please register an external provider or set {nameof(IMocaleConfiguration.UseExternalProvider)} to false.");
        }

        return mauiAppBuilder;
    }
}

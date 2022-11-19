using Mocale.Abstractions;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale;

/// <summary>
/// Host build extensions for Mocale
/// </summary>
public static class AppBuilderExtensions
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static MauiAppBuilder UseMocale(
        this MauiAppBuilder mauiAppBuilder,
        Action<MocaleBuilder> builder = default)
    {
        var mocaleBuilder = new MocaleBuilder()
        {
            AppBuilder = mauiAppBuilder, // Give the builders a reference so they can register things
        };

        // Invoke mocaleConfiguration action
        builder?.Invoke(mocaleBuilder);

        // Default config if the consumer doesn't call WithConfiguration(...)
        mocaleBuilder.ConfigurationManager ??= new ConfigurationManager<IMocaleConfiguration>(new MocaleConfiguration());

        mauiAppBuilder.Services.AddSingleton(mocaleBuilder.ConfigurationManager);
        mauiAppBuilder.Services.AddSingleton<ILocalizationManager, LocalizationManager>();

        // Is this really bad to be doing? probably 🙈
        ServiceProvider = mauiAppBuilder.Services.BuildServiceProvider();

        // Move to an init function so that this isn't ran in the host builder
        _ = ServiceProvider.GetRequiredService<ILocalizationManager>();

        return mauiAppBuilder;
    }
}

using Mocale.Abstractions;
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

        return mauiAppBuilder;
    }
}

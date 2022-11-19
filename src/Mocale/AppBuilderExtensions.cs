using Mocale.Abstractions;
using Mocale.Managers;
using Mocale.Models;
using Mocale.Services;

namespace Mocale
{
    /// <summary>
    /// Host build extensions for Mocale
    /// </summary>
    public static class AppBuilderExtensions
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static MauiAppBuilder UseMocale(
            this MauiAppBuilder builder,
            Action<MocaleBuilder>? configuration = default)
        {
            var configurationManager = new ConfigurationManager();
            builder.Services.AddSingleton<IConfigurationManager>(configurationManager);

            // Invoke configuration action
            configuration?.Invoke(new MocaleBuilder()
            {
                AppBuilder = builder, // Give the builders a reference so they can register things
                ConfigurationManager = configurationManager,
            });

            builder.Services.AddSingleton<ILocalizationManager, LocalizationManager>();

            // Is this really bad to be doing? probably 🙈
            ServiceProvider = builder.Services.BuildServiceProvider();

            // Make sure this is initialized before the app starts.
            // Maybe a feature to defer this init? to improve startup speeds...
            var man = ServiceProvider.GetRequiredService<ILocalizationManager>();

            return builder;
        }
    }
}

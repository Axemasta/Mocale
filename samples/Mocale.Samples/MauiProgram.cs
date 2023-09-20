using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Cache.SQLite;
using Mocale.Models;
using Mocale.Providers.GitHub.Raw;
using Mocale.Samples.ViewModels;
using Mocale.Samples.Views;
namespace Mocale.Samples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMocale(mocale =>
            {
                mocale.WithConfiguration(config =>
                    {
                        config.DefaultCulture = new CultureInfo("en-GB");
                        config.NotFoundSymbol = "?";
                    })
                    .UseSqliteCache(config =>
                    {
                        config.UpdateInterval = TimeSpan.FromSeconds(1);
                    })
                    .UseEmbeddedResources(config =>
                    {
                        config.ResourcesPath = "Locales";
                        config.ResourcesAssembly = typeof(App).Assembly;
                    })
                    .UseGitHubRaw(config =>
                    {
                        config.Username = "Axemasta";
                        config.Repository = "Mocale";
                        config.Branch = "mutliple-providers";
                        config.LocaleDirectory = "samples/Locales/";
                        config.ResourceFileDetails = new ResxResourceFileDetails()
                        {
                            ResourcePrefix = "AppResources",
                        };
                    });
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddLogging(logging =>
        {
#if DEBUG
            logging.AddDebug()
                .AddFilter("Mocale", LogLevel.Trace);
#endif
        });

        builder.Services.AddTransient<IntroductionPage>();
        builder.Services.AddTransient<IntroductionPageViewModel>();

        return builder.Build();
    }
}

using Microsoft.Extensions.Logging;
using Mocale.Providers.Azure.Blob;
using Mocale.Samples.Resources.Resx;

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
                    config.DefaultCulture = new System.Globalization.CultureInfo("en-GB");
                    config.NotFoundSymbol = "?";
                })
                //.UseAppResourcesLocalProvider(config =>
                //{
                //    config.AppResourcesType = typeof(AppResources);
                //})
                .UseJsonResourcesLocalProvider(config =>
                {
                    config.ResourcesPath = "Locales";
                    config.ResourcesAssembly = typeof(App).Assembly;
                })
                .UseBlobStorage(config =>
                {
                    config.BlobContainerUri = new Uri("https://azurestorage/mocale/");
                    config.RequiresAuthentication = false;
                    config.CheckForFile = true;
                });
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddLogging(builder =>
        {
#if DEBUG
            builder.AddDebug()
                .AddFilter("Mocale", LogLevel.Trace);
#endif
        });

        return builder.Build();
    }
}

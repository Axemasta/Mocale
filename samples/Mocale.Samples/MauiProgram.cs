using Microsoft.Extensions.Logging;
using Mocale.Json;

namespace Mocale.Samples;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }

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
                });

                // Would it be the worst idea to have usemocale & withconfig return a MocaleBuilder and then the WithResourceProvider return the host builder?

                //mocale.WithAppResourcesProvider(config =>
                //{
                //    config.AppResourcesType = typeof(AppResources);
                //});

                mocale.WithJsonResourcesProvider(config =>
                {
                    config.ResourcesPath = "Locales";
                    config.ResourcesAssembly = typeof(App).Assembly;
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

        // https://montemagno.com/dotnet-maui-appsettings-json-configuration/
        var app = builder.Build();
        Services = app.Services;
        return app;
    }
}

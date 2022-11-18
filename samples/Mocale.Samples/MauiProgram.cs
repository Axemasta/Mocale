using Microsoft.Extensions.Logging;
using Mocale.Enums;
using Mocale.Json;
using Mocale.Resx;
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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

using Microsoft.Extensions.Logging;
using Mocale.Enums;
using Mocale.Resx;

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
                    config.ResourceType = LocalResourceType.Resx;
                });

                // Would it be the worst idea to have usemocale & withconfig return a MocaleBuilder and then the WithResourceProvider return the host builder?

                mocale.WithAppResourcesProvider();
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

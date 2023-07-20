using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Providers.Azure.Blob;
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
                    //.UseAppResources(config =>
                    //{
                    //    config.AppResourcesType = typeof(AppResources);
                    //})
                    .UseEmbeddedResources(config =>
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
                //.UseS3Bucket(config =>
                //{
                //    config.BucketUri = new Uri("https://aws.com/mocale/");
                //});
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

using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Cache.SQLite;
using Mocale.Models;
using Mocale.Providers.GitHub.Raw;
using Mocale.Samples.ViewModels;
using Mocale.Samples.Pages;
using CommunityToolkit.Maui;
using Mocale.Samples.Enums;
using CommunityToolkit.Maui.Markup;
namespace Mocale.Samples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
#pragma warning disable CA1416
        builder
            .UseMauiApp<App>()
#pragma warning restore CA1416
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseMocale(mocale =>
            {
                mocale.WithConfiguration(config =>
                    {
                        config.DefaultCulture = new CultureInfo("en-GB");
                        config.NotFoundSymbol = "?";
                        config.EnumBehavior.OverrideRules.Add(typeof(Cities), new LocalizeEnumRule()
                        {
                            UseAttribute = true,
                            LocalizeAttribute = typeof(MocaleTranslationKeyAttribute),
                            AttributePropertyName = nameof(MocaleTranslationKeyAttribute.Key),
                        });
                    })
                    .UseSqliteCache(config =>
                    {
                        config.UpdateInterval = TimeSpan.FromDays(1);
                    })
                    .UseEmbeddedResources(config =>
                    {
                        config.ResourcesPath = "Locales";
                        config.ResourcesAssembly = typeof(App).Assembly;
                    })
                    //.UseGitHubRaw(config =>
                    //{
                    //    config.Username = "Axemasta";
                    //    config.Repository = "Mocale";
                    //    config.Branch = "main";
                    //    config.LocaleDirectory = "samples/Locales/";
                    //    config.ResourceFileDetails = new ResxResourceFileDetails()
                    //    {
                    //        ResourcePrefix = "AppResources",
                    //    };
                    //});
                    .UseGitHubRaw(config =>
                     {
                         config.Username = "Axemasta";
                         config.Repository = "Mocale";
                         config.Branch = "main";
                         config.LocaleDirectory = "samples/Locales/";
                         config.ResourceFileDetails = new JsonResourceFileDetails();
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

        builder.Services.AddTransient<BindingPage>();
        builder.Services.AddTransient<IntroductionPage>();
        builder.Services.AddTransient<ParameterPage>();

        builder.Services.AddTransient<IntroductionPageViewModel>();
        builder.Services.AddTransient<BindingViewModel>();
        builder.Services.AddTransient<ParameterViewModel>();

        return builder.Build();
    }
}

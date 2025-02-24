namespace Mocale;

internal class MocaleInitializeService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var localizationManager = services.GetRequiredService<ILocalizationManager>();
        var translatorManager = services.GetRequiredService<ITranslatorManager>();
        var configManager = services.GetRequiredService<IConfigurationManager<IMocaleConfiguration>>();

        MocaleLocator.MocaleConfiguration = configManager.Configuration;
        MocaleLocator.TranslatorManager = translatorManager;

        Task.Run(localizationManager.Initialize);
    }
}

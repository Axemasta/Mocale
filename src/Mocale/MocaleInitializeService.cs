namespace Mocale;

internal class MocaleInitializeService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var localizationManager = services.GetRequiredService<ILocalizationManager>();
        var translatorManager = services.GetRequiredService<ITranslatorManager>();

        MocaleLocator.TranslatorManager = translatorManager;

        Task.Run(localizationManager.Initialize);
    }
}

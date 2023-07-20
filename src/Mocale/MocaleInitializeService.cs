namespace Mocale;

internal class MocaleInitializeService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var localizationManager = services.GetRequiredService<ILocalizationManager>();

        MocaleLocator.LocalizationManager = localizationManager;

        // Unfortunately we need to block the thread to prevent any race conditions when initializing.
        // Sorry not sorry 😂🙈
        var t = localizationManager.Initialize();
        t.Wait();
    }
}

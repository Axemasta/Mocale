namespace Mocale;

internal class MocaleInitializeService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var localizationManager = services.GetRequiredService<ILocalizationManager>();

        MocaleLocator.LocalizationManager = localizationManager;

        Task.Run(localizationManager.Initialize);
    }
}

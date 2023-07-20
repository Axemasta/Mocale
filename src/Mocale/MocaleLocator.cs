using System.ComponentModel;
namespace Mocale;

/// <summary>
/// Mocale Service Locator for services that need to fallback to a service locator pattern.
/// The primary reason for this class is for XamlExtensions lacking access to the app's main service
/// provider.
/// See issue:
/// https://github.com/dotnet/maui/issues/8824
/// When the ability to peek the apps main service provider from markup extensions becomes available,
/// this service locator will be obsolete.
/// I've tried to make this not hang off statically stored instances as much as possible, at runtime
/// the app
/// will use the service provider and not store any state. During a test you can set a custom instance
/// to
/// mock any functionality required.
/// </summary>
public static class MocaleLocator
{
    /// <summary>
    /// Localization Manager
    /// </summary>
    public static ILocalizationManager LocalizationManager { get; internal set; }

    /// <summary>
    /// Set custom instance of <see cref="ILocalizationManager" /> for test scenarios
    /// </summary>
    /// <param name="instance"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetInstance(ILocalizationManager instance)
    {
        LocalizationManager = instance;
    }
}

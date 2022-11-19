using System.ComponentModel;
using Mocale.Abstractions;
using Mocale.Managers;

namespace Mocale;

/// <summary>
/// Mocale Service Locator for services that need to fallback to a service locator pattern
/// </summary>
public static class MocaleLocator
{
    private static ILocalizationManager customInstance;

    /// <summary>
    /// Get the current localization manager instance.
    /// <para/>
    /// Only use this locator if you class cannot acquire an instance of <see cref="ILocalizationManager"/> via
    /// constructor injection ie:
    /// <list type="bullet">
    /// <item><description>Converter</description></item>
    /// <item><description>Markup Extension</description></item>
    /// </list>
    /// etc
    /// </summary>
    /// <returns></returns>
    public static ILocalizationManager GetLocalizationManager()
    {
        // A future nice to have would be an analyzer to warn this usage unless the class is IValueConverter etc
        return customInstance ?? LocalizationManager.Instance;
    }

    /// <summary>
    /// Set custom instance of <see cref="ILocalizationManager"/> for test scenarios
    /// </summary>
    /// <param name="instance"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetInstance(ILocalizationManager instance)
    {
        customInstance = instance;
    }
}

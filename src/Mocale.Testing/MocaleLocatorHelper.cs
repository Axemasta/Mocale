using Mocale.Abstractions;

namespace Mocale.Testing;

/// <summary>
/// Mocale Locator Test Helper For Setting Internal Classes For Testing Purposes
/// </summary>
public static class MocaleLocatorHelper
{
    /// <summary>
    /// Set Translator Manager instance during testing
    /// </summary>
    /// <param name="manager"></param>
    public static void SetTranslatorManager(ITranslatorManager manager)
    {
        MocaleLocator.TranslatorManager = manager;
    }

    /// <summary>
    /// Set Mocale Configuration instance during testing, this is required when localizing enums
    /// </summary>
    /// <param name="configuration"></param>
    public static void SetMocaleConfiguration(IMocaleConfiguration configuration)
    {
        MocaleLocator.MocaleConfiguration = configuration;
    }
}

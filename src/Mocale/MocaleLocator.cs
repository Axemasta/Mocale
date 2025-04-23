using System.ComponentModel;
namespace Mocale;

/*
 * I'm not sure how to nullable refactor this service locator since its set in initialization so
 * should be marked nullable however I dislike the side effect on the rest of the code using this that i now need to check
 * the value before using it. This value will be set by the lib and always here, end of sentence :)
 */
#nullable disable

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
    public static ITranslatorManager TranslatorManager { get; internal set; }

    internal static IMocaleConfiguration MocaleConfiguration { get; set; }
}

#nullable enable

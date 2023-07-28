using System.Globalization;
namespace Mocale.Abstractions;

/// <summary>
/// Mocale Library Configuration
/// </summary>
public interface IMocaleConfiguration
{
    /// <summary>
    /// The type of localisation resource being used
    /// </summary>
    LocaleResourceType ResourceType { get; }

    /// <summary>
    /// The default culture to load
    /// </summary>
    CultureInfo DefaultCulture { get; }

    /// <summary>
    /// Whether missing keys should be shown on the UI
    /// </summary>
    bool ShowMissingKeys { get; }

    /// <summary>
    /// The wrapping symbol to use for missing keys
    /// </summary>
    string NotFoundSymbol { get; }

    /// <summary>
    /// Whether an external resource provider is in use
    /// </summary>
    bool UseExternalProvider { get; }

    /// <summary>
    /// Indicated whether culture changes are saved for use in future app sessions.
    /// </summary>
    bool SaveCultureChanged { get; }
}

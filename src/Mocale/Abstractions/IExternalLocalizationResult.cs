namespace Mocale.Abstractions;

/// <summary>
/// External Localization Result
/// </summary>
public interface IExternalLocalizationResult
{
    /// <summary>
    /// Whether the attempt succeeded
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Localizations from the external provider
    /// </summary>
    Dictionary<string, string>? Localizations { get; }
}

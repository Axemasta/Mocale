using System.Globalization;
namespace Mocale.Abstractions;

/// <summary>
/// Localization Manager
/// </summary>
public interface ILocalizationManager
{
    /// <summary>
    /// The current culture
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Mocale Bootstrapper, this is called internall by mocale during maui app startup
    /// </summary>
    /// <returns>Success</returns>
    Task<bool> Initialize();

    /// <summary>
    /// Set culture
    /// </summary>
    /// <param name="culture">The culture to attempt to display</param>
    /// <returns>Success</returns>
    Task<bool> SetCultureAsync(CultureInfo culture);
}

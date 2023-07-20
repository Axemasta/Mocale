using System.Globalization;
namespace Mocale.Abstractions;

/// <summary>
/// External Localization Provider - Provides localizations from an external source
/// </summary>
public interface IExternalLocalizationProvider
{
    /// <summary>
    /// Gets Values For Culture using the external source
    /// </summary>
    /// <param name="cultureInfo">The culture to attempt to load</param>
    /// <returns>External localization result for the given culture</returns>
    Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo);
}

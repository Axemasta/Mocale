using System.Globalization;

namespace Mocale.Abstractions;

/// <summary>
/// IInternalLocalizationProvider - Responsible for providing localizations from internally within
/// the application.
/// </summary>
public interface IInternalLocalizationProvider

{
    /// <summary>
    /// Gets the localization values for the given culture
    /// </summary>
    /// <param name="cultureInfo">The culture to get localizations for</param>
    /// <returns>The localizations as a key value pair</returns>
    Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo);
}

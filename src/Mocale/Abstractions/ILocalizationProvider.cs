using System.Globalization;

namespace Mocale.Abstractions
{
    /// <summary>
    /// ILocalizationProvider - Responsible for providing localizations
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Gets the localization values for the given culture
        /// </summary>
        /// <param name="cultureInfo">The culture to get localizations for</param>
        /// <returns>The localizations as a key value pair</returns>
        Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo);
    }
}

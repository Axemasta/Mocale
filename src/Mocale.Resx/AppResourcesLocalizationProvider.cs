using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Resx
{
    public class AppResourcesLocalizationProvider : ILocalizationProvider

    {
        // Great hacky implementation here, take notes recruiters!

        private Dictionary<CultureInfo, Dictionary<string, string>> Localizations { get; } = new Dictionary<CultureInfo, Dictionary<string, string>>()
        {
            {
                new CultureInfo("en-GB"),
                new Dictionary<string, string>()
                {
                    { "CurrentLocaleName", "English" },
                    { "MocaleDescription", "Localization framework for .NET Maui" },
                    { "MocaleTitle", "Mocale" },
                }
            },
            {
                new CultureInfo("fr-FR"),
                new Dictionary<string, string>()
                {
                    { "CurrentLocaleName", "French" },
                    { "MocaleDescription", "Framework de localisation pour .NET Maui" },
                    { "MocaleTitle", "Mocale" },
                }
            }
        };

        public Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo)
        {
            var kvp = Localizations[cultureInfo];

            if (kvp is not null)
            {
                return kvp;
            }

            return Localizations.First().Value;
        }
    }
}

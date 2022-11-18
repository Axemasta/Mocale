using System;
using System.Globalization;
using Mocale.Abstractions;
using Mocale.Json.Abstractions;

namespace Mocale.Json
{
    internal class JsonResourcesLocalizationProvider : ILocalizationProvider
    {
        private readonly IJsonResourcesConfig localConfig;

        public JsonResourcesLocalizationProvider(IJsonResourcesConfig jsonResourcesConfig)
        {
            this.localConfig = jsonResourcesConfig;
        }

        public Dictionary<string, string> GetValuesForCulture(CultureInfo cultureInfo)
        {
            var kvp = Localizations[cultureInfo];

            if (kvp != null)
            {
                return kvp;
            }

            return Localizations.First().Value;
        }

        private Dictionary<CultureInfo, Dictionary<string, string>> Localizations { get; } = new Dictionary<CultureInfo, Dictionary<string, string>>()
        {
            {
                new CultureInfo("en-GB"),
                new Dictionary<string, string>()
                {
                    { "CurrentLocaleName", "English" },
                    { "MocaleDescription", "Localization framework for .NET Maui, pulled from Json!" },
                    { "MocaleTitle", "Mocale" },
                }
            },
            {
                new CultureInfo("fr-FR"),
                new Dictionary<string, string>()
                {
                    { "CurrentLocaleName", "French" },
                    { "MocaleDescription", "Framework de localisation pour .NET Maui, tire de Json!" },
                    { "MocaleTitle", "Mocale" },
                }
            }
        };
    }
}


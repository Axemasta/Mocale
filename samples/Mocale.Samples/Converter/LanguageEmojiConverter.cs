using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Samples.Converter;

internal class LanguageEmojiConverter : IValueConverter
{
    private readonly ILocalizationManager localizationManager;

    public LanguageEmojiConverter()
    {
        localizationManager = MocaleLocator.GetLocalizationManager();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var currentCulture = localizationManager.CurrentCulture;

        return GetFlag(currentCulture.EnglishName);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public string GetFlag(string country)
    {
        // Adapted from
        // https://itnext.io/convert-country-name-to-flag-emoji-in-c-the-net-ecosystem-115f714d3ef9
        var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();
        var englishRegion = regions.FirstOrDefault(region => region.EnglishName.Contains(country));

        if (englishRegion == null)
        {
            return "🏳";
        }

        var region = new RegionInfo(englishRegion.LCID);

        var countryAbbrev = region.TwoLetterISORegionName;
        var flag = IsoCountryCodeToFlagEmoji(countryAbbrev);
        return flag;
    }

    public string IsoCountryCodeToFlagEmoji(string countryCode) => string.Concat(countryCode.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
}

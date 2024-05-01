using System.Globalization;

namespace Mocale.Samples.Converter;

internal sealed class LanguageEmojiConverter : IValueConverter
{
    private readonly ITranslatorManager translatorManager;

    public LanguageEmojiConverter()
    {
        translatorManager = MocaleLocator.TranslatorManager;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var currentCulture = translatorManager.CurrentCulture;

        if (currentCulture is null)
        {
            return null;
        }

        return GetFlag(currentCulture.EnglishName);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static string GetFlag(string country)
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

    private static string IsoCountryCodeToFlagEmoji(string countryCode)
    {
        return string.Concat(countryCode.ToUpperInvariant().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
    }
}

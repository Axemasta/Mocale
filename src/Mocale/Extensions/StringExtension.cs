using System.Globalization;
namespace Mocale.Extensions;

internal static class StringExtension
{
    public static bool TryParseCultureInfo(this string cultureString, out CultureInfo cultureInfo)
    {
        cultureInfo = null!;

        if (string.IsNullOrEmpty(cultureString) || !DoesCultureExist(cultureString))
        {
            return false;
        }

        cultureInfo = new CultureInfo(cultureString);
        return true;
    }

    // https://stackoverflow.com/a/16476935/8828057
    private static bool DoesCultureExist(string cultureName)
    {
        return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => string.Equals(culture.Name, cultureName, StringComparison.CurrentCultureIgnoreCase));
    }

    internal static string Reverse(string s)
    {
        // https://stackoverflow.com/a/228060/8828057
        var charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}

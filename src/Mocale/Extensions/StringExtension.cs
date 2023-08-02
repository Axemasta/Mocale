using System.Globalization;
namespace Mocale.Extensions;

internal static class StringExtension
{
    public static bool TryParseCultureInfo(this string cultureString, out CultureInfo cultureInfo)
    {
        cultureInfo = null!;

        if (string.IsNullOrEmpty(cultureString))
        {
            return false;
        }

        try
        {
            cultureInfo = new CultureInfo(cultureString);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static string Reverse(string s)
    {
        // https://stackoverflow.com/a/228060/8828057
        var charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}

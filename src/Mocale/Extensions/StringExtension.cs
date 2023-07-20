using System.Globalization;
namespace Mocale.Extensions;
#nullable enable
internal static class StringExtension
{
    public static bool TryParseCultureInfo(this string cultureString, out CultureInfo? cultureInfo)
    {
        cultureInfo = null;

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
}
#nullable disable

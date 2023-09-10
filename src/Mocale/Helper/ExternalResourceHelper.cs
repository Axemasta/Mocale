using System.Globalization;
namespace Mocale.Helper;

internal static class ExternalResourceHelper
{
    public static string GetExpectedJsonFileName(CultureInfo cultureInfo, string? versionPrefix)
    {
        var fileName = $"{cultureInfo.Name}.json";

        if (versionPrefix is null || string.IsNullOrEmpty(versionPrefix))
        {
            return fileName;
        }

        return string.Join("/", versionPrefix, fileName);
    }

    public static string GetExpectedResxFileName(CultureInfo cultureInfo, string? versionPrefix)
    {
        var fileName = $"{cultureInfo.Name}.resx";

        if (versionPrefix is null || string.IsNullOrEmpty(versionPrefix))
        {
            return fileName;
        }

        return string.Join("/", versionPrefix, fileName);
    }
}

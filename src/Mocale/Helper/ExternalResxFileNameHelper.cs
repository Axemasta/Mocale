using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Helper;
internal class ExternalResxFileNameHelper : IExternalFileNameHelper
{
    private readonly IVersionPrefixHelper versionPrefixHelper;

    public ExternalResxFileNameHelper(IVersionPrefixHelper versionPrefixHelper)
    {
        this.versionPrefixHelper = Guard.Against.Null(versionPrefixHelper, nameof(versionPrefixHelper));
    }

    public string GetExpectedFileName(CultureInfo culture)
    {
        var fileName = $"{culture.Name}.resx";

        return versionPrefixHelper.ApplyVersionPrefix(fileName);
    }
}

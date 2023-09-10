using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Helper;

internal class ExternalJsonFileNameHelper : IExternalFileNameHelper
{
    private readonly IVersionPrefixHelper versionPrefixHelper;

    public ExternalJsonFileNameHelper(IVersionPrefixHelper versionPrefixHelper)
    {
        this.versionPrefixHelper = Guard.Against.Null(versionPrefixHelper, nameof(versionPrefixHelper));
    }

    public string GetExpectedFileName(CultureInfo culture)
    {
        var fileName = $"{culture.Name}.json";

        return versionPrefixHelper.ApplyVersionPrefix(fileName);
    }
}

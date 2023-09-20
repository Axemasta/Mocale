using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Helper;

internal class ExternalResxFileNameHelper : IExternalFileNameHelper
{
    private readonly ResxResourceFileDetails resourceFileDetails;

    public ExternalResxFileNameHelper(IConfigurationManager<IExternalProviderConfiguration> configurationManager)
    {
        configurationManager = Guard.Against.Null(configurationManager, nameof(configurationManager));

        if (configurationManager.Configuration.ResourceFileDetails is not ResxResourceFileDetails fileDetails)
        {
            throw new NotSupportedException("Resource file details were not for resx files");
        }

        this.resourceFileDetails = fileDetails;
    }

    public string GetExpectedFileName(CultureInfo culture)
    {
        if (resourceFileDetails.PrimaryCulture == culture)
        {
            return $"{resourceFileDetails.ResourcePrefix}.resx";
        }

        var fileName = $"{resourceFileDetails.ResourcePrefix}.{culture.Name}.resx";

        if (!string.IsNullOrEmpty(resourceFileDetails.VersionPrefix))
        {
            fileName = string.Join("/", resourceFileDetails.VersionPrefix, fileName);
        }

        return fileName;
    }
}

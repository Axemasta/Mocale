using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Helper;

internal class ExternalJsonFileNameHelper : IExternalFileNameHelper
{
    private readonly JsonResourceFileDetails resourceFileDetails;

    public ExternalJsonFileNameHelper(IConfigurationManager<IExternalProviderConfiguration> configurationManager)
    {
        configurationManager = Guard.Against.Null(configurationManager, nameof(configurationManager));

        if (configurationManager.Configuration.ResourceFileDetails is not JsonResourceFileDetails fileDetails)
        {
            throw new NotSupportedException("Resource file details were not for json files");
        }

        this.resourceFileDetails = fileDetails;
    }

    public string GetExpectedFileName(CultureInfo culture)
    {
        var fileName = $"{culture.Name}.json";

        if (!string.IsNullOrEmpty(resourceFileDetails.VersionPrefix))
        {
            fileName = string.Join("/", resourceFileDetails.VersionPrefix, fileName);
        }

        return fileName;
    }
}

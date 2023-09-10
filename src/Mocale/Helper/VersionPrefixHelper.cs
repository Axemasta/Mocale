using Ardalis.GuardClauses;

namespace Mocale.Helper;
internal class VersionPrefixHelper : IVersionPrefixHelper
{
    private readonly IExternalConfiguration externalConfiguration;

    public VersionPrefixHelper(IConfigurationManager<IExternalConfiguration> externalConfigurationManager)
    {
        externalConfigurationManager = Guard.Against.Null(externalConfigurationManager, nameof(externalConfigurationManager));

        this.externalConfiguration = externalConfigurationManager.Configuration;
    }

    public string ApplyVersionPrefix(string fileName)
    {
        if (string.IsNullOrEmpty(externalConfiguration.VersionPrefix))
        {
            return fileName;
        }

        return string.Join("/", externalConfiguration.VersionPrefix, fileName);
    }
}

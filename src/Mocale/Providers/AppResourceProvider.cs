using System.Collections;
using System.Globalization;
using System.Resources;
using Ardalis.GuardClauses;
using Mocale.Exceptions;

namespace Mocale.Providers;

internal partial class AppResourceProvider : IInternalLocalizationProvider
{
    private readonly ILogger logger;
    private readonly IMocaleConfiguration mocaleConfiguration;
    private readonly ResourceManager resourceManager;

    public AppResourceProvider(
        IConfigurationManager<IAppResourcesConfig> appResourcesConfigurationManager,
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        ILogger<AppResourceProvider> logger)
    {
        this.logger = Guard.Against.Null(logger);

        var appResourcesConfig = appResourcesConfigurationManager.Configuration;
        mocaleConfiguration = mocaleConfigurationManager.Configuration;

        if (appResourcesConfig.AppResourcesType is null)
        {
            throw new InitializationException("App Resource Type has not been set, this should be configured during startup");
        }

        resourceManager = new ResourceManager(appResourcesConfig.AppResourcesType);
    }

    public Dictionary<string, string>? GetValuesForCulture(CultureInfo cultureInfo)
    {
        // https://stackoverflow.com/a/1970941/8828057
        var resourceSet = resourceManager.GetResourceSet(cultureInfo, true, false);

        if (resourceSet is null)
        {
            // The default culture will not have a name (ie en-GB.resx) so we will have to trust the
            // user configured this correctly?!
            if (cultureInfo.Equals(mocaleConfiguration.DefaultCulture))
            {
                var defaultSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);

                if (defaultSet is null)
                {
                    LogUnableToLoadDefaultResourceSet();
                    return null;
                }

                return ConvertResourceSet(defaultSet);
            }

            LogNoResourcesFoundForCultureCultureName(cultureInfo.Name);
            return null;
        }

        return ConvertResourceSet(resourceSet);
    }

    private static Dictionary<string, string> ConvertResourceSet(ResourceSet resourceSet)
    {
        return resourceSet
            .OfType<DictionaryEntry>()
            .ToDictionary(r =>
                    r.Key.ToString() ?? string.Empty,
                r => r.Value?.ToString() ?? string.Empty);
    }

    #region Logging

    [LoggerMessage(LogLevel.Warning, "Unable to load default resource set")]
    partial void LogUnableToLoadDefaultResourceSet();

    [LoggerMessage(LogLevel.Warning, "No resources found for culture {CultureName}")]
    partial void LogNoResourcesFoundForCultureCultureName(string cultureName);

    #endregion Logging
}

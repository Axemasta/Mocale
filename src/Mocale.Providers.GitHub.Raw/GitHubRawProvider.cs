using System.Globalization;
using Ardalis.GuardClauses;
using Mocale.Providers.GitHub.Raw.Helpers;
namespace Mocale.Providers.GitHub.Raw;

internal class GitHubRawProvider : IExternalLocalizationProvider
{
    #region Fields

    private readonly IGithubRawConfig githubConfig;
    private readonly IExternalFileNameHelper externalFileNameHelper;
    private readonly ILocalizationParser localizationParser;
    private readonly ILogger logger;

    private readonly HttpClient httpClient;

    #endregion Fields

    #region Constructors

    public GitHubRawProvider(
        IConfigurationManager<IGithubRawConfig> githubConfigurationManager,
        IExternalFileNameHelper externalFileNameHelper,
        HttpClient httpClient,
        ILocalizationParser localizationParser,
        ILogger<GitHubRawProvider> logger)
    {
        githubConfigurationManager = Guard.Against.Null(githubConfigurationManager, nameof(githubConfigurationManager));

        this.githubConfig = githubConfigurationManager.Configuration;
        this.externalFileNameHelper = Guard.Against.Null(externalFileNameHelper, nameof(externalFileNameHelper));
        this.httpClient = Guard.Against.Null(httpClient, nameof(httpClient));
        this.localizationParser = Guard.Against.Null(localizationParser, nameof(localizationParser));
        this.logger = Guard.Against.Null(logger, nameof(logger));
    }

    #endregion Constructors

    private Uri GetResourceUrl(CultureInfo cultureInfo)
    {
        var fileName = externalFileNameHelper.GetExpectedFileName(cultureInfo);

        return RawUrlBuilder.BuildResourceUrl(githubConfig.Username, githubConfig.Repository, githubConfig.Branch, githubConfig.LocaleDirectory, fileName);
    }

    private async Task<IExternalLocalizationResult> QueryResourceUrlForLocalizations(Uri resourceUri)
    {
        try
        {
            var response = await httpClient.GetAsync(resourceUri);

            if (!response.IsSuccessStatusCode)
            {
                // Handle Error
                logger.LogWarning("Api call failed with status code: {StatusCode}, for resource url: {ResourceUrl}", (int)response.StatusCode, resourceUri);

                return new ExternalLocalizationResult()
                {
                    Success = false,
                };
            }

            await using var resourceStream = await response.Content.ReadAsStreamAsync();

            var localizations = localizationParser.ParseLocalizationStream(resourceStream);

            if (localizations is null)
            {
                logger.LogWarning($"Api call succeeded but resource could not be deserialized as {nameof(Dictionary<string, string>)}");

                return new ExternalLocalizationResult()
                {
                    Success = false,
                };
            }

            return new ExternalLocalizationResult()
            {
                Success = true,
                Localizations = localizations,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred quering raw resource: {ResourceUrl}", resourceUri);

            return new ExternalLocalizationResult()
            {
                Success = false,
            };
        }
    }

    public async Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
    {
        var fileName = externalFileNameHelper.GetExpectedFileName(cultureInfo);

        var resourceUrl = RawUrlBuilder.BuildResourceUrl(githubConfig.Username, githubConfig.Repository, githubConfig.Branch, githubConfig.LocaleDirectory, fileName);

        return await QueryResourceUrlForLocalizations(resourceUrl);
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Mocale.Helper;
using Mocale.Providers.GitHub.Raw.Helpers;
namespace Mocale.Providers.GitHub.Raw;

internal class GitHubRawProvider : IExternalLocalizationProvider
{
    #region Fields

    private readonly IGithubRawConfig githubConfig;
    private readonly ILogger logger;

    private readonly HttpClient httpClient;

    #endregion Fields

    #region Constructors

    public GitHubRawProvider(
        IConfigurationManager<IGithubRawConfig> githubConfigurationManager,
        IHttpClientFactory httpClientFactory,
        ILogger<GitHubRawProvider> logger)
    {
        githubConfigurationManager = Guard.Against.Null(githubConfigurationManager, nameof(githubConfigurationManager));

        this.githubConfig = githubConfigurationManager.Configuration;
        this.logger = Guard.Against.Null(logger, nameof(logger));

        httpClient = httpClientFactory.CreateClient("Mocale.Providers.GitHub.Raw");
    }

    #endregion Constructors

    private Uri GetResourceUrl(CultureInfo cultureInfo)
    {
        var fileName = ExternalResourceHelper.GetExpectedJsonFileName(cultureInfo, null);

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

            var localizations = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(resourceStream);

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
        var fileName = ExternalResourceHelper.GetExpectedJsonFileName(cultureInfo, null);

        var resourceUrl = RawUrlBuilder.BuildResourceUrl(githubConfig.Username, githubConfig.Repository, githubConfig.Branch, githubConfig.LocaleDirectory, fileName);

        return await QueryResourceUrlForLocalizations(resourceUrl);
    }
}

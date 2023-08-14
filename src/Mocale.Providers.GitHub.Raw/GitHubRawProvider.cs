using System.Globalization;
using Ardalis.GuardClauses;
using Mocale.Helper;
namespace Mocale.Providers.GitHub.Raw;

internal class GitHubRawProvider : IExternalLocalizationProvider
{
    #region Fields

    private readonly IGithubRawConfig githubConfig;
    private readonly ILogger logger;

    #endregion Fields

    #region Constructors

    public GitHubRawProvider(
        IConfigurationManager<IGithubRawConfig> githubConfigurationManager,
        ILogger<GitHubRawProvider> logger)
    {
        githubConfigurationManager = Guard.Against.Null(githubConfigurationManager, nameof(githubConfigurationManager));

        this.githubConfig = githubConfigurationManager.Configuration;
        this.logger = Guard.Against.Null(logger, nameof(logger));
    }

    #endregion Constructors

    public async Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
    {
        var fileName = ExternalResourceHelper.GetExpectedJsonFileName(cultureInfo, null);

        var baseUrl = "https://raw.githubusercontent.com/Axemasta/Mocale/github-provider/samples/Locales/";

        var fullUrl = Path.Combine(baseUrl, fileName);

        using var httpClient = new HttpClient();

        try
        {
            var response = await httpClient.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Handle Error
            }

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new ExternalLocalizationResult()
        {
            Success = false,
        };
    }
}

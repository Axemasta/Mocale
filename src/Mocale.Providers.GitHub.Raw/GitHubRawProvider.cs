using System.Globalization;
using Ardalis.GuardClauses;
using Mocale.Providers.GitHub.Raw.Helpers;

namespace Mocale.Providers.GitHub.Raw;

internal partial class GitHubRawProvider : IExternalLocalizationProvider
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
		githubConfigurationManager = Guard.Against.Null(githubConfigurationManager);

		githubConfig = githubConfigurationManager.Configuration;
		this.externalFileNameHelper = Guard.Against.Null(externalFileNameHelper);
		this.httpClient = Guard.Against.Null(httpClient);
		this.localizationParser = Guard.Against.Null(localizationParser);
		this.logger = Guard.Against.Null(logger);
	}

	#endregion Constructors

	private async Task<IExternalLocalizationResult> QueryResourceUrlForLocalizations(Uri resourceUri)
	{
		try
		{
			var response = await httpClient.GetAsync(resourceUri);

			if (!response.IsSuccessStatusCode)
			{
				// Handle Error
				LogApiCallFailedWithStatusCodeStatusCodeForResourceUrlResourceUrl((int)response.StatusCode, resourceUri);

				return new ExternalLocalizationResult
				{
					Success = false,
				};
			}

			await using var resourceStream = await response.Content.ReadAsStreamAsync();

			var localizations = localizationParser.ParseLocalizationStream(resourceStream);

			if (localizations is null)
			{
				LogApiCallSucceededButResourceCouldNotBeDeserializedAsDictionaryStringString();

				return new ExternalLocalizationResult
				{
					Success = false,
				};
			}

			return new ExternalLocalizationResult
			{
				Success = true,
				Localizations = localizations,
			};
		}
		catch (Exception ex)
		{
			LogAnExceptionOccurredQueringRawResourceResourceUrl(ex, resourceUri);

			return new ExternalLocalizationResult
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

	#region Logging

	[LoggerMessage(LogLevel.Warning, "Api call failed with status code: {StatusCode}, for resource url: {ResourceUrl}")]
	partial void LogApiCallFailedWithStatusCodeStatusCodeForResourceUrlResourceUrl(int statusCode, Uri resourceUrl);

	[LoggerMessage(LogLevel.Warning, "Api call succeeded but resource could not be deserialized as Dictionary<string, string>")]
	partial void LogApiCallSucceededButResourceCouldNotBeDeserializedAsDictionaryStringString();

	[LoggerMessage(LogLevel.Error, "An exception occurred quering raw resource: {ResourceUrl}")]
	partial void LogAnExceptionOccurredQueringRawResourceResourceUrl(Exception exception, Uri resourceUrl);

	#endregion Logging
}

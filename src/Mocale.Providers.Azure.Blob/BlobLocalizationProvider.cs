using Mocale.Extensions;

namespace Mocale.Providers.Azure.Blob;

internal sealed partial class BlobLocalizationProvider : IExternalLocalizationProvider
{
    #region Fields

    private readonly IBlobStorageConfig blobStorageConfig;
    private readonly IBlobResourceLocator blobResourceLocator;
    private readonly IExternalFileNameHelper externalFileNameHelper;
    private readonly ILocalizationParser localizationParser;
    private readonly ILogger logger;

    private static readonly BlobOpenReadOptions BlobOptions = new(false);

    #endregion Fields

    #region Constructors

    public BlobLocalizationProvider(
        IBlobResourceLocator blobResourceLocator,
        IConfigurationManager<IBlobStorageConfig> blobConfigurationManager,
        IExternalFileNameHelper externalFileNameHelper,
        ILocalizationParser localizationParser,
        ILogger<BlobLocalizationProvider> logger)
    {
        this.blobResourceLocator = Guard.Against.Null(blobResourceLocator, nameof(blobResourceLocator));
        this.externalFileNameHelper = Guard.Against.Null(externalFileNameHelper, nameof(externalFileNameHelper));
        this.localizationParser = Guard.Against.Null(localizationParser, nameof(localizationParser));
        this.logger = Guard.Against.Null(logger, nameof(logger));

        blobConfigurationManager = Guard.Against.Null(blobConfigurationManager, nameof(blobConfigurationManager));
        blobStorageConfig = blobConfigurationManager.Configuration;
    }

    #endregion Constructors

    #region Methods

    private async Task<Uri?> GetResourceUrl(CultureInfo cultureInfo)
    {
        string fileName;

        if (!blobStorageConfig.CheckForFile)
        {
            fileName = externalFileNameHelper.GetExpectedFileName(cultureInfo);
        }
        else
        {
            var blobDetails = await blobResourceLocator.TryLocateResource(cultureInfo);

            if (!blobDetails.Exists || string.IsNullOrEmpty(blobDetails.ResourceName))
            {
                return null;
            }

            fileName = blobDetails.ResourceName;
        }

        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        return blobStorageConfig.BlobContainerUri.TryAppend(out var resourceUri, fileName)
            ? resourceUri
            : null;
    }

    private async Task<Dictionary<string, string>?> RetrieveAndParseFileContents(Uri blobUrl)
    {
        try
        {
            var blobClient = new BlobClient(blobUrl);

            await using var blobStream = await blobClient.OpenReadAsync(BlobOptions);

            if (blobStream is null)
            {
                LogBlobStreamWasNullForUrlBlobUrl(blobUrl);
                return null;
            }

            return localizationParser.ParseLocalizationStream(blobStream);
        }
        catch (Exception ex)
        {
            LogAnExceptionOccurredRetrievingParsingFileFromBlobStorageBlobUrl(ex, blobUrl);
            return null;
        }
    }

    #endregion Methods

    #region Interface Implementations

    public async Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
    {
        var resourceUrl = await GetResourceUrl(cultureInfo);

        if (resourceUrl is null)
        {
            LogUnableToFileFileThatMatchesCultureNameCultureName(cultureInfo.Name);

            // File could not be found
            return new ExternalLocalizationResult()
            {
                Success = false,
            };
        }

        var fileContents = await RetrieveAndParseFileContents(resourceUrl);

        if (fileContents is null)
        {
            LogUnableToReadFileContentsAsLocalizationsResourceUrl(resourceUrl);

            return new ExternalLocalizationResult()
            {
                Success = false,
            };
        }

        return new ExternalLocalizationResult()
        {
            Success = true,
            Localizations = fileContents,
        };
    }

    #endregion Interface Implementations

    #region Logging

    [LoggerMessage(LogLevel.Warning, "Blob stream was null for url: {BlobUrl}")]
    partial void LogBlobStreamWasNullForUrlBlobUrl(Uri blobUrl);

    [LoggerMessage(LogLevel.Error, "An exception occurred retrieving & parsing file from blob storage: {BlobUrl}")]
    partial void LogAnExceptionOccurredRetrievingParsingFileFromBlobStorageBlobUrl(Exception exception, Uri blobUrl);

    [LoggerMessage(LogLevel.Warning, "Unable to file file that matches culture name: {CultureName}")]
    partial void LogUnableToFileFileThatMatchesCultureNameCultureName(string cultureName);

    [LoggerMessage(LogLevel.Warning, "Unable to read file contents as localizations: {ResourceUrl}")]
    partial void LogUnableToReadFileContentsAsLocalizationsResourceUrl(Uri resourceUrl);

    #endregion Logging
}

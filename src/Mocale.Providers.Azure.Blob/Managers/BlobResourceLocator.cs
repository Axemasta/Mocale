using System.Globalization;
using Ardalis.GuardClauses;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Page = Azure.Page<Azure.Storage.Blobs.Models.BlobItem>;
namespace Mocale.Providers.Azure.Blob.Managers;

internal class BlobResourceLocator : IBlobResourceLocator
{
    #region Fields

    private readonly IBlobStorageConfig blobStorageConfig;
    private readonly IExternalFileNameHelper externalFileNameHelper;
    private readonly ILogger logger;

    #endregion Fields

    #region Constructors

    public BlobResourceLocator(
        IConfigurationManager<IBlobStorageConfig> blobConfigurationManager,
        IExternalFileNameHelper externalFileNameHelper,
        ILogger<BlobResourceLocator> logger)
    {
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.externalFileNameHelper = Guard.Against.Null(externalFileNameHelper, nameof(externalFileNameHelper));

        blobConfigurationManager = Guard.Against.Null(blobConfigurationManager, nameof(blobConfigurationManager));
        blobStorageConfig = blobConfigurationManager.Configuration;
    }

    #endregion Constructors

    #region Methods

    private static bool FindMatchingBlobResource(IReadOnlyList<BlobItem> blobItems, string fileSlug, out BlobResourceInfo? blobInfo)
    {
        blobInfo = default;

        foreach (var blobItem in blobItems)
        {
            if (!blobItem.Name.Equals(fileSlug, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            blobInfo = new BlobResourceInfo()
            {
                Exists = true,
                ResourceName = blobItem.Name,
            };

            return true;
        }

        return false;
    }

    #endregion Methods

    #region Interface Implementations

    /// <inheritdoc/>
    public async Task<BlobResourceInfo> TryLocateResource(CultureInfo cultureInfo)
    {
        try
        {
            var expectedFileSlug = externalFileNameHelper.GetExpectedFileName(cultureInfo);

            var client = new BlobContainerClient(blobStorageConfig.BlobContainerUri);

            var pages = client.GetBlobsAsync().AsPages();

            await foreach (Page page in pages)
            {
                if (FindMatchingBlobResource(page.Values, expectedFileSlug, out var info) && info is not null)
                {
                    return info;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred location resource for culture: {CultureName}", cultureInfo.Name);
        }

        return new BlobResourceInfo
        {
            Exists = false,
        };
    }

    #endregion Interface Implementations
}

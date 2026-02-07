namespace Mocale.Providers.Azure.Blob.Models;

/// <inheritdoc/>
public class BlobStorageConfig : IBlobStorageConfig
{
    /// <summary>
    /// The default blob container URI used when no URI is configured.
    /// </summary>
    internal const string DefaultBlobContainerUri = "app://mocale";

    /// <inheritdoc/>
    public Uri BlobContainerUri { get; set; } = new Uri(DefaultBlobContainerUri); // Default value so we don't have to mark as nullable

    /// <inheritdoc/>
    public bool RequiresAuthentication { get; set; }

    /// <inheritdoc/>
    public bool CheckForFile { get; set; } = true;

    /// <inheritdoc/>
    public IResourceFileDetails ResourceFileDetails { get; set; } = new JsonResourceFileDetails();

    /// <inheritdoc/>
    public bool UseETagCaching { get; set; } = true;
}

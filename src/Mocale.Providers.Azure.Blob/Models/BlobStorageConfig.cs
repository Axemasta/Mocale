namespace Mocale.Providers.Azure.Blob.Models;

/// <inheritdoc/>
public class BlobStorageConfig : IBlobStorageConfig
{
    /// <inheritdoc/>
    public Uri BlobContainerUri { get; set; } = new Uri("app://mocale"); // Default value so we don't have to mark as nullable

    /// <inheritdoc/>
    public bool RequiresAuthentication { get; set; }

    /// <inheritdoc/>
    public bool CheckForFile { get; set; } = true;

    /// <inheritdoc/>
    public IResourceFileDetails ResourceFileDetails { get; set; } = new JsonResourceFileDetails();
}

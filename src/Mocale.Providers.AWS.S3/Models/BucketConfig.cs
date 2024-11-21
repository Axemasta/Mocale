namespace Mocale.Providers.AWS.S3.Models;

/// <inheritdoc/>
public class BucketConfig : IBucketConfig
{
    /// <inheritdoc/>
    public Uri? BucketUri { get; set; }

    /// <inheritdoc/>
    public IResourceFileDetails ResourceFileDetails { get; set; } = new JsonResourceFileDetails();
}

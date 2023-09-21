namespace Mocale.Providers.AWS.S3.Models;

public class BucketConfig : IBucketConfig
{
    public Uri? BucketUri { get; set; }

    /// <inheritdoc/>
    public IResourceFileDetails ResourceFileDetails { get; set; } = new JsonResourceFileDetails();
}

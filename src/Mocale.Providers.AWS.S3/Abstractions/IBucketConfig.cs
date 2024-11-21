namespace Mocale.Providers.AWS.S3.Abstractions;

/// <summary>
/// S3 Bucket Config
/// </summary>
public interface IBucketConfig : IExternalProviderConfiguration
{
    /// <summary>
    /// Uri for the blob container
    /// </summary>
    Uri? BucketUri { get; }
}

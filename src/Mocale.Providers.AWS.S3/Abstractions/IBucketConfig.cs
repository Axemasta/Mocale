using Mocale.Abstractions;

namespace Mocale.Providers.AWS.S3.Abstractions;

public interface IBucketConfig : IExternalProviderConfiguration
{
    /// <summary>
    /// Uri for the blob container
    /// </summary>
    Uri? BucketUri { get; }
}

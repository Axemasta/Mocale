using Mocale.Providers.AWS.S3.Abstractions;

namespace Mocale.Providers.AWS.S3.Models;

public class BucketConfig : IBucketConfig
{
    public Uri? BucketUri { get; set; }
}
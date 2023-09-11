using Mocale.Enums;

namespace Mocale.Providers.AWS.S3.Models;

public class BucketConfig : IBucketConfig
{
    public Uri? BucketUri { get; set; }

    public LocaleResourceType ResourceType { get; set; } = LocaleResourceType.Json;

    public string? VersionPrefix { get; set; }
}

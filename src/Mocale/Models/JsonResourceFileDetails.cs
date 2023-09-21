namespace Mocale.Models;

public class JsonResourceFileDetails : IResourceFileDetails
{
    /// <inheritdoc/>
    public LocaleResourceType ResourceType { get; } = LocaleResourceType.Json;
    /// <inheritdoc/>
    public string? VersionPrefix { get; set; }
}

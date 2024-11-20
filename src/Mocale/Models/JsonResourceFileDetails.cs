namespace Mocale.Models;

/// <inheritdoc/>
public class JsonResourceFileDetails : IResourceFileDetails
{
    /// <inheritdoc/>
    public LocaleResourceType ResourceType { get; } = LocaleResourceType.Json;
    /// <inheritdoc/>
    public string? VersionPrefix { get; set; }
}

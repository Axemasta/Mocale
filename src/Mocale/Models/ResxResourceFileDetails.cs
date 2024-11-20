using System.Globalization;

namespace Mocale.Models;

/// <inheritdoc/>
public class ResxResourceFileDetails : IResourceFileDetails
{
    /// <inheritdoc/>
    public LocaleResourceType ResourceType { get; } = LocaleResourceType.Resx;

    /// <inheritdoc/>
    public string? VersionPrefix { get; set; }

    /// <summary>
    /// The name of the resource file
    /// </summary>
    public required string ResourcePrefix { get; set; }

    /// <summary>
    /// The primary resources culture (if this exists). This will not have the culture suffix in its file path.
    /// </summary>
    public CultureInfo? PrimaryCulture { get; set; }
}

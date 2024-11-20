namespace Mocale.Abstractions;

/// <summary>
/// Resource File Details
/// </summary>
public interface IResourceFileDetails
{
    /// <summary>
    /// The type of file being stored externally
    /// </summary>
    LocaleResourceType ResourceType { get; }

    /// <summary>
    /// The version prefix of the files, this will be prepended after the url &amp; before the file name
    /// </summary>
    string? VersionPrefix { get; }
}

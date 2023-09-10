namespace Mocale.Abstractions;

public interface IExternalConfiguration
{
    /// <summary>
    /// The type of file being stored externally
    /// </summary>
    LocaleResourceType ResourceType { get; }

    /// <summary>
    /// The version prefix of the files, this will be prepended after the url & before the file name
    /// </summary>
    string? VersionPrefix { get; }
}

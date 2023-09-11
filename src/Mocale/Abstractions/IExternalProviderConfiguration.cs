namespace Mocale.Abstractions;

public interface IExternalProviderConfiguration

{
    /// <summary>
    /// The type of file being stored externally
    /// </summary>
    LocaleResourceType ResourceType { get; }

    /// <summary>
    /// The version prefix of the files, this will be prepanded after the url &amp; before the file name
    /// </summary>
    string? VersionPrefix { get; }
}

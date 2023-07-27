using Mocale.Enums;
namespace Mocale.Providers.Azure.Blob.Abstractions;

public interface IBlobStorageConfig
{
    /// <summary>
    /// Uri for the blob container
    /// </summary>
    Uri BlobContainerUri { get; }

    /// <summary>
    /// Whether the requests require authentication.
    /// <para />
    /// If true then the authentication host builder extension will need to be provided in order for
    /// the configured authenticaton to kick in
    /// </summary>
    bool RequiresAuthentication { get; }

    /// <summary>
    /// Whether the blob provider should check for a file before attempting to download it. This
    /// will require an extra api call but reduce exception noise.
    /// </summary>
    bool CheckForFile { get; }

    /// <summary>
    /// Folder prefix for the bloc container, this will be added to the blob
    /// </summary>
    string? VersionPrefix { get; }

    /// <summary>
    /// The type of file stored externally
    /// </summary>
    LocaleResourceType FileType { get; }
}

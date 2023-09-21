namespace Mocale.Providers.Azure.Blob.Abstractions;

public interface IBlobStorageConfig : IExternalProviderConfiguration
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
}

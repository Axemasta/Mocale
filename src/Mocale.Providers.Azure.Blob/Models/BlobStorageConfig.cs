namespace Mocale.Providers.Azure.Blob.Models;

public class BlobStorageConfig : IBlobStorageConfig
{
    public Uri? BlobContainerUri { get; set; }

    public bool RequiresAuthentication { get; set; }

    public bool CheckForFile { get; set; } = true;
}

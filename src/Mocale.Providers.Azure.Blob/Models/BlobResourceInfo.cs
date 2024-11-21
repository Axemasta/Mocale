namespace Mocale.Providers.Azure.Blob.Models;

/// <summary>
/// Blob Resource Information
/// </summary>
public class BlobResourceInfo
{
    /// <summary>
    /// Whether the theorized blob resource actually exists
    /// </summary>
    public bool Exists { get; init; }

    /// <summary>
    /// The name of the theorized blob resource
    /// </summary>
    public string? ResourceName { get; init; }
}

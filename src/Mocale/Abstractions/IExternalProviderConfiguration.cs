namespace Mocale.Abstractions;

/// <summary>
/// External Provider Configuration
/// </summary>
public interface IExternalProviderConfiguration
{
    /// <summary>
    /// Details about the resource files returned by the external provider
    /// </summary>
    IResourceFileDetails ResourceFileDetails { get; }
}


namespace Mocale.Abstractions;

/// <summary>
/// TODO:
/// </summary>
public interface IEtagCompatibleProviderConfiguration
{
    /// <summary>
    /// Gets or sets whether ETag caching is enabled for this provider.
    /// When enabled, the provider will use HTTP ETags to avoid downloading unchanged resources.
    /// </summary>
    bool UseETagCaching { get; set; }
}

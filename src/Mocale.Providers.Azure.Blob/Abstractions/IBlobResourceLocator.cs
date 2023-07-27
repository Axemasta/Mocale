using System.Globalization;
namespace Mocale.Providers.Azure.Blob.Abstractions;

/// <summary>
/// Blob Resource Locator
/// </summary>
public interface IBlobResourceLocator
{
    /// <summary>
    /// Try to locate blob resource for the given culture
    /// </summary>
    /// <param name="cultureInfo">The culture to try locate blob resource for</param>
    /// <returns>Result of location</returns>
    Task<BlobResourceInfo> TryLocateResource(CultureInfo cultureInfo);
}

using System.Globalization;

namespace Mocale.Abstractions;

/// <summary>
/// Manages storage and retrieval of ETags for locale resources.
/// </summary>
public interface IETagCacheManager
{
    /// <summary>
    /// Gets the stored ETag for the specified culture.
    /// </summary>
    /// <param name="cultureInfo">The culture to get the ETag for.</param>
    /// <returns>The stored ETag, or null if no ETag exists for this culture.</returns>
    string? GetETag(CultureInfo cultureInfo);

    /// <summary>
    /// Stores an ETag for the specified culture.
    /// </summary>
    /// <param name="cultureInfo">The culture to store the ETag for.</param>
    /// <param name="etag">The ETag value to store.</param>
    /// <returns>True if the ETag was successfully stored, false otherwise.</returns>
    bool SaveETag(CultureInfo cultureInfo, string etag);

    /// <summary>
    /// Removes the stored ETag for the specified culture.
    /// </summary>
    /// <param name="cultureInfo">The culture to remove the ETag for.</param>
    /// <returns>True if the ETag was successfully removed, false otherwise.</returns>
    bool RemoveETag(CultureInfo cultureInfo);
}

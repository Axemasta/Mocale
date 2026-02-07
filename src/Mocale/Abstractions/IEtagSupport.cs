using System.Globalization;

namespace Mocale.Abstractions;

/// <summary>
/// Interface for external providers that support ETag-based caching.
/// Implement this interface alongside IExternalLocalizationProvider to enable ETag caching.
/// </summary>
internal interface IETagSupport
{
    /// <summary>
    /// Gets localizations for the specified culture, using ETag for conditional requests.
    /// </summary>
    /// <param name="cultureInfo">The culture to get localizations for.</param>
    /// <param name="etag">The ETag from a previous request, or null if no cached version exists.</param>
    /// <returns>A result containing the localizations and the new ETag, or indicating no changes if the resource hasn't been modified.</returns>
    Task<ETagLocalizationResult> GetValuesForCultureWithETagAsync(CultureInfo cultureInfo, string? etag);
}

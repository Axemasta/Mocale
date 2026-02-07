namespace Mocale.Models;

/// <summary>
/// Represents the result of a localization request that supports ETag caching.
/// </summary>
internal class ETagLocalizationResult
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Indicates whether the resource was modified since the last request.
    /// When false, the cached version is still valid.
    /// </summary>
    public bool WasModified { get; init; } = true;

    /// <summary>
    /// The localizations returned from the provider.
    /// Will be null if WasModified is false (304 Not Modified response).
    /// </summary>
    public Dictionary<string, string>? Localizations { get; init; }

    /// <summary>
    /// The ETag returned from the server for this resource.
    /// Store this value to use in subsequent requests.
    /// </summary>
    public string? ETag { get; init; }

    /// <summary>
    /// Creates a result indicating the resource was not modified (304 response).
    /// </summary>
    public static ETagLocalizationResult NotModified() => new()
    {
        Success = true,
        WasModified = false,
        Localizations = null,
        ETag = null
    };

    /// <summary>
    /// Creates a result indicating the request failed.
    /// </summary>
    public static ETagLocalizationResult Failed() => new()
    {
        Success = false,
        WasModified = true,
        Localizations = null,
        ETag = null
    };
}

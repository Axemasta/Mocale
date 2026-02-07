using Mocale.Managers;
namespace Mocale;

/// <summary>
/// Mocale Builder
/// </summary>
public class MocaleBuilder
{
    /// <summary>
    /// Maui App Builder
    /// </summary>
    public required MauiAppBuilder AppBuilder { get; init; }

    /// <summary>
    /// Configuration Manager
    /// </summary>
    public required ConfigurationManager<IMocaleConfiguration> ConfigurationManager { get; init; }

    internal string? LocalProviderName { get; set; }

    internal bool LocalProviderRegistered { get; set; }

    internal string? ExternalProviderName { get; set; }

    internal bool ExternalProviderRegistered { get; set; }

    internal IExternalProviderConfiguration? ExternalProviderConfiguration { get; set; }

    internal bool CacheProviderRegistered { get; set; }

    /// <summary>
    /// Action to register the ETag cache manager for the current cache provider.
    /// Set by the cache provider during registration.
    /// </summary>
    internal Action<IServiceCollection>? RegisterETagCacheManager { get; set; }

    /// <summary>
    /// Use Mocale With Given Configuration
    /// </summary>
    /// <param name="configureMocale">Action to configure mocale</param>
    /// <returns>Mocale builder</returns>
    /// <exception cref="InvalidCastException"></exception>
    public MocaleBuilder WithConfiguration(Action<MocaleConfiguration> configureMocale)
    {
        // I do this so that IMocaleConfiguration preserves its immutability with only getters, little jank...
        var config = ConfigurationManager.Configuration as MocaleConfiguration
            ?? throw new InvalidCastException($"Unable to cast {nameof(IMocaleConfiguration)} as {nameof(MocaleConfiguration)}");

        configureMocale.Invoke(config);

        return this;
    }
}

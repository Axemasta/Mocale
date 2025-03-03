using Ardalis.GuardClauses;

namespace Mocale.Managers;

/// <summary>
/// Simple Singleton Style Class For Storing Configuration Objects
/// </summary>
/// <typeparam name="TConfig"></typeparam>
public class ConfigurationManager<TConfig>(TConfig config)
    : IConfigurationManager<TConfig>, IConfigurationUpdateManager<TConfig>
{
    /// <inheritdoc/>
    public TConfig Configuration { get; } = Guard.Against.Null(config, nameof(config));

    /// <inheritdoc/>
    public void UpdateConfiguration(Action<TConfig> configuration)
    {
        configuration(Configuration);
    }
}

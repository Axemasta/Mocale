namespace Mocale.Abstractions;

/// <summary>
/// Configuration Manager
/// </summary>
/// <typeparam name="TConfig">The config for this manager</typeparam>
public interface IConfigurationManager<TConfig>
{
    /// <summary>
    /// The configuration for the given area
    /// </summary>
    TConfig Configuration { get; set; }
}

/// <summary>
/// Configuration Update Manager
/// </summary>
/// <typeparam name="TConfig"></typeparam>
public interface IConfigurationUpdateManager<TConfig>
{
    /// <summary>
    /// Update Configuration
    /// </summary>
    /// <param name="configuration"></param>
    void UpdateConfiguration(Action<TConfig> configuration);
}

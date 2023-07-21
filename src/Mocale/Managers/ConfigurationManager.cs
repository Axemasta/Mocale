using Ardalis.GuardClauses;

namespace Mocale.Managers;

/// <summary>
/// Simple Singleton Style Class For Storing Configuration Objects
/// </summary>
/// <typeparam name="TConfig"></typeparam>
public class ConfigurationManager<TConfig> : IConfigurationManager<TConfig>, IConfigurationUpdateManager<TConfig>
{
    public TConfig Configuration { get; set; }

    public ConfigurationManager(TConfig config)
    {
        Configuration = Guard.Against.Null(config, nameof(config));
    }

    public void UpdateConfiguration(Action<TConfig> configuration)
    {
        configuration(Configuration);
    }
}

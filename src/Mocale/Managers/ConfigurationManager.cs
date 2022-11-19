using Ardalis.GuardClauses;
using Mocale.Abstractions;

namespace Mocale.Managers;

/// <summary>
/// Simple Singleton Style Class For Storing Configuration Objects
/// </summary>
/// <typeparam name="TConfig"></typeparam>
internal class ConfigurationManager<TConfig> : IConfigurationManager<TConfig>, IConfigurationUpdateManager<TConfig>
{
    private TConfig config;

    internal ConfigurationManager(TConfig config)
    {
        SetConfiguration(config);
    }

    public TConfig GetConfiguration()
    {
        return config;
    }

    public void SetConfiguration(TConfig configuration)
    {
        this.config = Guard.Against.Null(configuration, nameof(configuration));
    }

    public void UpdateConfiguration(Action<TConfig> configuration)
    {
        configuration(this.config);
    }
}

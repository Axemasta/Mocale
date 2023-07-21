namespace Mocale.Abstractions;

public interface IConfigurationManager<TConfig>
{
    TConfig Configuration { get; set; }
}

public interface IConfigurationUpdateManager<TConfig>
{
    void UpdateConfiguration(Action<TConfig> configuration);
}

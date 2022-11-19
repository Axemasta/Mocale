namespace Mocale.Abstractions;

public interface IConfigurationManager<T>
{
    T GetConfiguration();

    void SetConfiguration(T configuration);
}

public interface IConfigurationUpdateManager<TConfig>
{
    void UpdateConfiguration(Action<TConfig> configuration);
}

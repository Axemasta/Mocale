namespace Mocale.Abstractions;

/// <summary>
/// Configuration Manager
/// </summary>
public interface IConfigurationManager : IConfigurationManager<IMocaleConfiguration>
{
}

public interface IConfigurationManager<T>
{
    T GetConfiguration();

    void SetConfiguration(T configuration);
}
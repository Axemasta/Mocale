namespace Mocale.Abstractions;

public interface IConfigurationManager<T>
{
    T GetConfiguration();

    void SetConfiguration(T configuration);
}

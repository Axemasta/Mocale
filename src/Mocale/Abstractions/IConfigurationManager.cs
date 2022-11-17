namespace Mocale.Abstractions;

/// <summary>
/// Configuration Manager
/// </summary>
internal interface IConfigurationManager
{
    IMocaleConfiguration GetConfiguration();

    void SetConfiguration(IMocaleConfiguration mocaleConfiguration);
}

using Mocale.Abstractions;
using Mocale.Models;

namespace Mocale.Services;

internal class ConfigurationManager : IConfigurationManager
{
    private IMocaleConfiguration? configuration;

    public static IConfigurationManager Instance { get; } = new ConfigurationManager();

    public IMocaleConfiguration GetConfiguration()
    {
        return configuration ?? new MocaleConfiguration();
    }

    public void SetConfiguration(IMocaleConfiguration mocaleConfiguration)
    {
        this.configuration = mocaleConfiguration;
    }
}

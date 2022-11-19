using Mocale.Abstractions;
using Mocale.Managers;
using Mocale.Models;

namespace Mocale;

public class MocaleBuilder
{
    public ILocalizationProvider LocalizationProvider { get; set; }

    internal MauiAppBuilder AppBuilder { get; set; }

    internal ConfigurationManager<IMocaleConfiguration>? ConfigurationManager { get; set; }

    public MocaleBuilder WithConfiguration(Action<MocaleConfiguration> configuration)
    {
        var config = new MocaleConfiguration();

        configuration.Invoke(config);

        ConfigurationManager = new ConfigurationManager<IMocaleConfiguration>(config);

        return this;
    }
}

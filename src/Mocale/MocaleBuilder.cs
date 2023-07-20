using Mocale.Managers;
namespace Mocale;

public class MocaleBuilder
{
    public IInternalLocalizationProvider LocalizationProvider { get; set; }

    internal MauiAppBuilder AppBuilder { get; set; }

    internal ConfigurationManager<IMocaleConfiguration>? ConfigurationManager { get; set; }

    internal string LocalProviderName { get; set; }

    internal bool LocalProviderRegistered { get; set; }

    internal string ExternalProviderName { get; set; }

    internal bool ExternalProviderRegistered { get; set; }

    public MocaleBuilder WithConfiguration(Action<MocaleConfiguration> configureMocale)
    {
        var config = new MocaleConfiguration();

        configureMocale.Invoke(config);

        ConfigurationManager = new ConfigurationManager<IMocaleConfiguration>(config);

        return this;
    }
}

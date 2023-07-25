using Mocale.Managers;
namespace Mocale;

public class MocaleBuilder
{
    public IInternalLocalizationProvider? LocalizationProvider { get; set; }

    public required MauiAppBuilder AppBuilder { get; set; }

    public required ConfigurationManager<IMocaleConfiguration> ConfigurationManager { get; set; }

    internal string? LocalProviderName { get; set; }

    internal bool LocalProviderRegistered { get; set; }

    internal string? ExternalProviderName { get; set; }

    internal bool ExternalProviderRegistered { get; set; }

    internal bool CacheProviderRegistered { get; set; }

    public MocaleBuilder WithConfiguration(Action<MocaleConfiguration> configureMocale)
    {
        var config = ConfigurationManager.Configuration as MocaleConfiguration
            ?? throw new InvalidCastException($"Unable to cast {nameof(IMocaleConfiguration)} as {nameof(MocaleConfiguration)}");

        configureMocale.Invoke(config);

        return this;
    }
}

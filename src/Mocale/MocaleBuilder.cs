using Mocale.Abstractions;
using Mocale.Models;
using Mocale.Services;

namespace Mocale
{
    public class MocaleBuilder
    {
        public static MocaleBuilder Instance { get; } = new();

        public ILocalizationProvider LocalizationProvider { get; set; }

        public MocaleBuilder WithConfiguration(Action<MocaleConfiguration> configuration)
        {
            var config = new MocaleConfiguration();
            configuration.Invoke(config);
            ConfigurationManager.Instance.SetConfiguration(config);

            return this;
        }

        public MocaleBuilder WithLocalizationProvider(Func<ILocalizationProvider> configuration)
        {
            var provider = configuration();

            if (provider is null)
            {
                throw new ArgumentNullException("A provider is required", nameof(provider));
            }

            Instance.LocalizationProvider = provider;

            return this;
        }
    }
}

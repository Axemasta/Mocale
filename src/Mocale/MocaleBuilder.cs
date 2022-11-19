using Mocale.Abstractions;
using Mocale.Models;
using Mocale.Services;

namespace Mocale
{
    public class MocaleBuilder
    {
        public ILocalizationProvider LocalizationProvider { get; set; }

        internal MauiAppBuilder AppBuilder { get; set; }

        internal IConfigurationManager ConfigurationManager { get; set; }

        public MocaleBuilder WithConfiguration(Action<MocaleConfiguration> configuration)
        {
            var config = new MocaleConfiguration();
            configuration.Invoke(config);

            ConfigurationManager.SetConfiguration(config);

            return this;
        }

        public MocaleBuilder WithLocalizationProvider(Func<ILocalizationProvider> configuration)
        {
            // This probably isn't needed now

            return this;
        }
    }
}

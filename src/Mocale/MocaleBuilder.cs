using Mocale.Abstractions;
using Mocale.Models;

namespace Mocale
{
    public class MocaleBuilder
    {
        public static MocaleBuilder Instance { get; } = new();

        public ILocalizationProvider LocalizationProvider { get; set; }

        public MocaleBuilder WithConfiguration(Action<MocaleConfiguration> configuration)
        {
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

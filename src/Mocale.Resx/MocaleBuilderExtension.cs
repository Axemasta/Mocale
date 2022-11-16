namespace Mocale.Resx
{
    public static class MocaleBuilderExtension
    {
        public static MocaleBuilder WithAppResourcesProvider(this MocaleBuilder builder)
        {
            var provider = new AppResourcesLocalizationProvider();

            return builder.WithLocalizationProvider(() => provider);
        }
    }
}

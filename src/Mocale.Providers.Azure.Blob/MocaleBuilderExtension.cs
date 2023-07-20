using Mocale.Abstractions;
using Mocale.Managers;
namespace Mocale.Providers.Azure.Blob;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseBlobStorage(this MocaleBuilder builder, Action<BlobStorageConfig> configureBlobStorage)
    {
        builder.RegisterExternalProvider(typeof(BlobLocalizationProvider));

        var config = new BlobStorageConfig();
        configureBlobStorage(config);

        var configurationManager = new ConfigurationManager<IBlobStorageConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IBlobStorageConfig>>(configurationManager);
        builder.AppBuilder.Services.AddSingleton<IExternalLocalizationProvider, BlobLocalizationProvider>();

        return builder;
    }
}

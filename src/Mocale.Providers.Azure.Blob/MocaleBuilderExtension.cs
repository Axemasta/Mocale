using Mocale.Abstractions;
using Mocale.Managers;
using Mocale.Providers.Azure.Blob.Abstractions;
using Mocale.Providers.Azure.Blob.Models;

namespace Mocale.Providers.Azure.Blob;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseBlobStorage(this MocaleBuilder builder, Action<BlobStorageConfig> configureBlobStorage)
    {
        var config = new BlobStorageConfig();
        configureBlobStorage(config);

        var configurationManager = new ConfigurationManager<IBlobStorageConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IBlobStorageConfig>>(configurationManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationSource, BlobLocalizationProvider>();

        return builder;
    }
}

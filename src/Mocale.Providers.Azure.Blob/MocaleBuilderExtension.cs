using Mocale.Exceptions;
using Mocale.Managers;
using Mocale.Providers.Azure.Blob.Managers;
namespace Mocale.Providers.Azure.Blob;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseBlobStorage(this MocaleBuilder builder, Action<BlobStorageConfig> configureBlobStorage)
    {
        builder.RegisterExternalProvider(typeof(BlobLocalizationProvider));

        var config = new BlobStorageConfig();
        configureBlobStorage(config);

        if (config.BlobContainerUri.OriginalString.Equals("app://mocale", StringComparison.Ordinal))
        {
            throw new InitializationException("You must set a blob container uri to use this provider");
        }

        builder.RegisterExternalResourceFileTypeResources(config);

        // TODO: Is there a way I can avoid having 2 of the same config managers?
        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IBlobStorageConfig>>(new ConfigurationManager<IBlobStorageConfig>(config));
        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IExternalConfiguration>>(new ConfigurationManager<IExternalConfiguration>(config));
        builder.AppBuilder.Services.AddSingleton<IExternalLocalizationProvider, BlobLocalizationProvider>();
        builder.AppBuilder.Services.AddSingleton<IBlobResourceLocator, BlobResourceLocator>();

        return builder;
    }
}

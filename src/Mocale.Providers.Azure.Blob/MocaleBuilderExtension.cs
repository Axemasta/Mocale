using Mocale.Exceptions;
using Mocale.Providers.Azure.Blob.Managers;
namespace Mocale.Providers.Azure.Blob;

public static class MocaleBuilderExtension
{
    /// <summary>
    /// [External Provider]
    /// Use Azure Blob Storage to retrieve localizations
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureBlobStorage"></param>
    /// <returns></returns>
    /// <exception cref="InitializationException"></exception>
    public static MocaleBuilder UseBlobStorage(this MocaleBuilder builder, Action<BlobStorageConfig> configureBlobStorage)
    {
        var config = new BlobStorageConfig();
        configureBlobStorage(config);

        builder.RegisterExternalProvider<IBlobStorageConfig>(typeof(BlobLocalizationProvider), config);

        if (config.BlobContainerUri.OriginalString.Equals("app://mocale", StringComparison.Ordinal))
        {
            throw new InitializationException("You must set a blob container uri to use this provider");
        }

        builder.AppBuilder.Services.AddSingleton<IExternalLocalizationProvider, BlobLocalizationProvider>();
        builder.AppBuilder.Services.AddSingleton<IBlobResourceLocator, BlobResourceLocator>();

        return builder;
    }
}

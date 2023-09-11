using Mocale.Abstractions;
using Mocale.Managers;
namespace Mocale.Providers.AWS.S3;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseS3Bucket(this MocaleBuilder builder, Action<BucketConfig> configureBucket)
    {
        var config = new BucketConfig();
        configureBucket(config);

        builder.RegisterExternalProvider<IBucketConfig>(typeof(S3BucketProvider), config);

        var configurationManager = new ConfigurationManager<IBucketConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IBucketConfig>>(configurationManager);
        builder.AppBuilder.Services.AddSingleton<IExternalLocalizationProvider, S3BucketProvider>();

        return builder;
    }
}

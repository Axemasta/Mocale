using Mocale.Abstractions;
using Mocale.Managers;
using Mocale.Providers.AWS.S3.Abstractions;
using Mocale.Providers.AWS.S3.Models;

namespace Mocale.Providers.AWS.S3;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseS3Bucket(this MocaleBuilder builder, Action<BucketConfig> configureBucket)
    {
        var config = new BucketConfig();
        configureBucket(config);

        var configurationManager = new ConfigurationManager<IBucketConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IBucketConfig>>(configurationManager);
        builder.AppBuilder.Services.AddSingleton<ILocalizationSource, S3BucketProvider>();

        return builder;
    }
}

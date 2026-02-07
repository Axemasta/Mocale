using Microsoft.Extensions.DependencyInjection.Extensions;
using Mocale.Cache;

namespace Mocale.Extensions;

internal static class IEtagCompatibleProviderConfigurationExtension
{
    public static void RegisterEtagCacheManagerIfNeeded(this IEtagCompatibleProviderConfiguration configuration, MocaleBuilder builder)
    {
        if (!configuration.UseETagCaching)
        {
            return;
        }

        if (builder.RegisterETagCacheManager is not null)
        {
            builder.RegisterETagCacheManager(builder.AppBuilder.Services);
        }
        else
        {
            builder.AppBuilder.Services.TryAddSingleton<IETagCacheManager, InMemoryETagCacheManager>();
        }
    }
}

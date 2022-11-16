using Mocale.Models;

namespace Mocale
{
    /// <summary>
    /// Host build extensions for Mocale
    /// </summary>
    public static class AppBuilderExtensions
    {
        public static MauiAppBuilder UseConfiguredMocale(
            this MauiAppBuilder builder,
            Action<MocaleConfiguration>? configuration = default)
        {
            // Invoke configuration action
            configuration?.Invoke(new MocaleConfiguration());

            return builder;
        }

        public static MauiAppBuilder UseMocale(
            this MauiAppBuilder builder,
            Action<MocaleBuilder>? configuration = default)
        {
            // Invoke configuration action
            configuration?.Invoke(new MocaleBuilder());

            return builder;
        }
    }
}

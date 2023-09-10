using Mocale.Managers;

namespace Mocale.Providers.GitHub.Raw;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseGitHubRaw(this MocaleBuilder builder, Action<GithubRawConfig> configureGithub, Action<HttpClient>? configureHttpClient = null)
    {
        builder.RegisterExternalProvider(typeof(GitHubRawProvider));

        var config = new GithubRawConfig();
        configureGithub(config);

        builder.RegisterExternalResourceFileTypeResources(config);

        // TODO: Is there a way I can avoid having 2 of the same config managers?
        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IGithubRawConfig>>(new ConfigurationManager<IGithubRawConfig>(config));
        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IExternalConfiguration>>(new ConfigurationManager<IExternalConfiguration>(config));
        builder.AppBuilder.Services.AddSingleton<IExternalLocalizationProvider, GitHubRawProvider>();

        if (configureHttpClient is not null)
        {
            builder.AppBuilder.Services.AddHttpClient<IExternalLocalizationProvider, GitHubRawProvider>(configureHttpClient);
        }
        else
        {
            builder.AppBuilder.Services.AddHttpClient<IExternalLocalizationProvider, GitHubRawProvider>();
        }

        return builder;
    }
}

using Mocale.Managers;

namespace Mocale.Providers.GitHub.Raw;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseGitHubRaw(this MocaleBuilder builder, Action<GithubRawConfig> configureGithub, Action<HttpClient>? configureHttpClient = null)
    {
        builder.RegisterExternalProvider(typeof(GitHubRawProvider));

        var config = new GithubRawConfig();
        configureGithub(config);

        var configurationManager = new ConfigurationManager<IGithubRawConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IGithubRawConfig>>(configurationManager);
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

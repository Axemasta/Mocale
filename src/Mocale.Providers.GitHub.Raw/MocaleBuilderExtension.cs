using Mocale.Managers;

namespace Mocale.Providers.GitHub.Raw;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseGitHubRaw(this MocaleBuilder builder, Action<GithubRawConfig> configureGithub)
    {
        builder.RegisterExternalProvider(typeof(GitHubRawProvider));

        var config = new GithubRawConfig();
        configureGithub(config);

        var configurationManager = new ConfigurationManager<IGithubRawConfig>(config);

        builder.AppBuilder.Services.AddSingleton<IConfigurationManager<IGithubRawConfig>>(configurationManager);
        builder.AppBuilder.Services.AddSingleton<IExternalLocalizationProvider, GitHubRawProvider>();
        builder.AppBuilder.Services.AddHttpClient();

        return builder;
    }
}

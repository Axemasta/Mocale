namespace Mocale.Providers.GitHub.Raw;

public static class MocaleBuilderExtension
{
    /// <summary>
    /// [External Provider]
    /// Use GitHub Raw API to retrieve localizations.
    /// </summary>
    /// <param name="builder">Mocale Builder</param>
    /// <param name="configureGithub">Configuration Action</param>
    /// <param name="configureHttpClient">HTTP Client Configuration Action</param>
    /// <returns>Mocale Builder</returns>
    public static MocaleBuilder UseGitHubRaw(this MocaleBuilder builder, Action<GithubRawConfig> configureGithub, Action<HttpClient>? configureHttpClient = null)
    {
        var config = new GithubRawConfig();
        configureGithub(config);

        builder.RegisterExternalProvider<IGithubRawConfig>(typeof(GitHubRawProvider), config);

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

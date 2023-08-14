namespace Mocale.Providers.GitHub.Raw;

public static class MocaleBuilderExtension
{
    public static MocaleBuilder UseGitHubRawProvider(this MocaleBuilder builder, Action<GithubRawConfig> configureGithub)
    {
        return builder;
    }
}

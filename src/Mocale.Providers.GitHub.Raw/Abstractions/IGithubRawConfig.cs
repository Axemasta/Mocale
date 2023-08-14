namespace Mocale.Providers.GitHub.Raw.Abstractions;

public interface IGithubRawConfig
{
    string Username { get; }

    string Repository { get; }

    string Branch { get; }

    string? LocaleDirectory { get; }
}

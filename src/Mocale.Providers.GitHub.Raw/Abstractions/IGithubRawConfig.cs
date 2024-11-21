namespace Mocale.Providers.GitHub.Raw.Abstractions;

/// <summary>
/// External provider configuration for github raw source
/// </summary>
public interface IGithubRawConfig : IExternalProviderConfiguration
{
    /// <summary>
    /// The github username
    /// </summary>
    string Username { get; }

    /// <summary>
    /// The repository to target
    /// </summary>
    string Repository { get; }

    /// <summary>
    /// The branch the localizations exist on
    /// </summary>
    string Branch { get; }

    /// <summary>
    /// The directory in the repository that contains the locale files
    /// </summary>
    string LocaleDirectory { get; }
}

namespace Mocale.Providers.GitHub.Raw.Models;

/// <inheritdoc/>
public class GithubRawConfig : IGithubRawConfig
{
    /// <inheritdoc/>
    public string Username { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string Repository { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string Branch { get; set; } = "main";

    /// <inheritdoc/>
    public string LocaleDirectory { get; set; } = string.Empty;

    /// <inheritdoc/>
    public IResourceFileDetails ResourceFileDetails { get; set; } = new JsonResourceFileDetails();
}

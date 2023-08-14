namespace Mocale.Providers.GitHub.Raw.Models;

public class GithubRawConfig
{
    public string Username { get; set; } = string.Empty;

    public string Repository { get; set; } = string.Empty;

    public string Branch { get; set; } = "main";

    public string? LocaleDirectory { get; set; }
}

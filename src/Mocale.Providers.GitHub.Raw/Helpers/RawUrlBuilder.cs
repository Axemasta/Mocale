using Mocale.Extensions;
namespace Mocale.Providers.GitHub.Raw.Helpers;

internal static class RawUrlBuilder
{
    public static Uri BuildResourceUrl(string username, string repository, string branch, string filePath, string fileName)
    {
        // https://raw.githubusercontent.com/Axemasta/Mocale/github-provider/samples/Locales/fr-FR.json
        return new Uri("https://raw.githubusercontent.com/")
            .Append(username, repository, branch, filePath, fileName);
    }
}

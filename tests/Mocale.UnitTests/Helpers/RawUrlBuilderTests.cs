
using Mocale.Providers.GitHub.Raw.Helpers;
namespace Mocale.UnitTests.Helpers;

public class RawUrlBuilderTests
{
    [Theory]
    [InlineData(
        "Axemasta", "Mocale",
        "main", "samples/Locales",
        "en-GB.json", "https://raw.githubusercontent.com/Axemasta/Mocale/main/samples/Locales/en-GB.json")]
    [InlineData(
        "Axemasta", "Mocale",
        "main", "samples/Locales",
        "fr-FR.json", "https://raw.githubusercontent.com/Axemasta/Mocale/main/samples/Locales/fr-FR.json")]
    [InlineData(
        "Axemasta", "Mocale",
        "develop", "samples/Locales",
        "en-GB.json", "https://raw.githubusercontent.com/Axemasta/Mocale/develop/samples/Locales/en-GB.json")]
    [InlineData(
        "Axemasta", "Mocale",
        "develop", "samples/Locales",
        "fr-FR.json", "https://raw.githubusercontent.com/Axemasta/Mocale/develop/samples/Locales/fr-FR.json")]
    public void TryBuildResourceUrl_WhenProvidedValues_ShouldReturnSuccess(string username, string repository, string branch, string filePath, string fileName, string expectedUrl)
    {
        // Arrange

        // Act
        var result = RawUrlBuilder.BuildResourceUrl(username, repository, branch, filePath, fileName);

        // Assert
        Assert.Equal(expectedUrl, result.ToString());
    }
}

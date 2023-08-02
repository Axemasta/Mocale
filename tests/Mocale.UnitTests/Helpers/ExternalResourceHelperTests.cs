using System.Globalization;
using Mocale.Helper;

namespace Mocale.UnitTests.Helpers;

public class ExternalResourceHelperTests
{
    [Theory]
    [InlineData("en-GB", "en-GB.json")]
    [InlineData("fr-FR", "fr-FR.json")]
    public void GetExpectedJsonFileName_WhenVersionPrefixIsNull_ShouldAppendJson(string cultureString, string expectedFileName)
    {
        // Arrange
        var culture = new CultureInfo(cultureString);

        // Act
        var fileName = ExternalResourceHelper.GetExpectedJsonFileName(culture, null);

        // Assert
        Assert.Equal(expectedFileName, fileName);
    }

    [Theory]
    [InlineData("en-GB", "en-GB.json")]
    [InlineData("fr-FR", "fr-FR.json")]
    public void GetExpectedJsonFileName_WhenVersionPrefixIsEmpty_ShouldAppendJson(string cultureString, string expectedFileName)
    {
        // Arrange
        var culture = new CultureInfo(cultureString);

        // Act
        var fileName = ExternalResourceHelper.GetExpectedJsonFileName(culture, string.Empty);

        // Assert
        Assert.Equal(expectedFileName, fileName);
    }

    [Theory]
    [InlineData("en-GB", "v1", "v1/en-GB.json")]
    [InlineData("fr-FR", "v1", "v1/fr-FR.json")]
    [InlineData("en-GB", "v2", "v2/en-GB.json")]
    [InlineData("fr-FR", "v2", "v2/fr-FR.json")]
    public void GetExpectedJsonFileName_WhenVersionPrefixIsProvided_ShouldAppendVersionAndJson(string cultureString, string prefix, string expectedFileName)
    {
        // Arrange
        var culture = new CultureInfo(cultureString);

        // Act
        var fileName = ExternalResourceHelper.GetExpectedJsonFileName(culture, prefix);

        // Assert
        Assert.Equal(expectedFileName, fileName);
    }
}

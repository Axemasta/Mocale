using Mocale.Extensions;

namespace Mocale.UnitTests.Extensions;

public class StringExtensionTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void TryParseCultureInfo_WhenInputIsNullOrEmpty_ShouldReturnFalse(string invalidInput)
    {
        // Arrange

        // Act
        var result = invalidInput.TryParseCultureInfo(out var cultureInfo);

        // Assert
        Assert.False(result);
        Assert.Null(cultureInfo);
    }

    [Theory]
    [InlineData("en-GB", true)]
    [InlineData("en-US", true)]
    [InlineData("fr-FR", true)]
    [InlineData("HELLO WORLD", false)]
    [InlineData("ThIs-IsNot_A-CulTuR3", false)]
    public void TryParseCultureInfo_WhenProvidedInput_ShouldParseCorrectly(string cultureString, bool expectedValid)
    {
        // Arrange

        // Act
        var result = cultureString.TryParseCultureInfo(out var cultureInfo);

        // Assert
        Assert.Equal(expectedValid, result);

        if (expectedValid)
        {
            Assert.NotNull(cultureInfo);
        }
        else
        {
            Assert.Null(cultureInfo);
        }
    }
}


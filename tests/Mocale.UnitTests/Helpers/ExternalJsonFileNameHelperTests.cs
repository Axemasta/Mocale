using System.Globalization;
using Mocale.Abstractions;
using Mocale.Helper;

namespace Mocale.UnitTests.Helpers;

public class ExternalJsonFileNameHelperTests : FixtureBase
{
    #region Setup

    private readonly Mock<IVersionPrefixHelper> versionPrefixHelper;

    public ExternalJsonFileNameHelperTests()
    {
        versionPrefixHelper = new Mock<IVersionPrefixHelper>();
    }

    public override object CreateSystemUnderTest()
    {
        return new ExternalJsonFileNameHelper(versionPrefixHelper.Object);
    }

    #endregion Setup

    #region Tests

    [Theory]
    [InlineData("en-GB", "en-GB.json")]
    [InlineData("fr-FR", "fr-FR.json")]
    public void GetExpectedFileName_ShouldCreateCorrectFileName_AndPassToPrefixer(string cultureString, string expectedFileName)
    {
        // Arrange
        var culture = new CultureInfo(cultureString);

        var sut = GetSut<IExternalFileNameHelper>();

        versionPrefixHelper.Setup(m => m.ApplyVersionPrefix(expectedFileName))
            .Returns(expectedFileName);

        // Act
        var fileName = sut.GetExpectedFileName(culture);

        // Assert
        Assert.Equal(expectedFileName, fileName);
        versionPrefixHelper.Verify();
    }

    #endregion Tests
}

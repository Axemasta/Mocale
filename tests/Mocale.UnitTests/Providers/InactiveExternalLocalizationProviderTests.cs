using System.Globalization;
using Mocale.Abstractions;
using Mocale.Providers;

namespace Mocale.UnitTests.Providers;

public class InactiveExternalLocalizationProviderTests : FixtureBase<IExternalLocalizationProvider>
{
    #region Setup

    public override IExternalLocalizationProvider CreateSystemUnderTest()
    {
        return new InactiveExternalLocalizationProvider();
    }

    #endregion Setup

    #region Tests

    [Fact]
    public async Task GetValuesForCultureAsync_WhenCalled_ShouldReturnBlankResult()
    {
        var loadResult = await Sut.GetValuesForCultureAsync(new CultureInfo("en-GB"));

        Assert.False(loadResult.Success);
        Assert.Null(loadResult.Localizations);
    }

    #endregion Tests
}

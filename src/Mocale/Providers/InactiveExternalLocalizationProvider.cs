using System.Globalization;
namespace Mocale.Providers;

internal sealed class InactiveExternalLocalizationProvider : IExternalLocalizationProvider
{
    public Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
    {
        IExternalLocalizationResult blankResult = new ExternalLocalizationResult()
        {
            Success = false,
        };

        return Task.FromResult(blankResult);
    }
}

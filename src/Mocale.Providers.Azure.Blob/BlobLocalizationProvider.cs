using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Providers.Azure.Blob;

internal class BlobLocalizationProvider : IExternalLocalizationProvider
{
    public Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }
}

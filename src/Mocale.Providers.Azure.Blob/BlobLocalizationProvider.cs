using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Providers.Azure.Blob;

internal class BlobLocalizationProvider : ILocalizationSource
{
    public Task<Dictionary<string, string>> GetValuesForCulture(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }
}

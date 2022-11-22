using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Providers.AWS.S3;

internal class S3BucketProvider : ILocalizationSource
{
    public Task<Dictionary<string, string>> GetValuesForCulture(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }
}

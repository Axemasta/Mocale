using System.Globalization;
using Mocale.Abstractions;

namespace Mocale.Providers.AWS.S3;

internal class S3BucketProvider : IExternalLocalizationProvider
{
    public Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }
}
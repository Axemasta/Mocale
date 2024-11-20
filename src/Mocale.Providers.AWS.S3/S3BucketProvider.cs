namespace Mocale.Providers.AWS.S3;

internal sealed class S3BucketProvider : IExternalLocalizationProvider
{
#pragma warning disable CS1998
    public async Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
#pragma warning restore CS1998
    {
        if (cultureInfo.ToString() == "en-GB")
        {
            return new ExternalLocalizationResult()
            {
                Success = true,
                Localizations = new Dictionary<string, string>()
                {
                    { "CurrentLocaleName", "S3English" },
                    { "LocalizationCurrentProviderIs", "S3The current localization provider is:" },
                    { "LocalizationProviderName", "S3Json" },
                    { "MocaleDescription", "S3Localization framework for .NET Maui" },
                    { "MocaleTitle", "S3Mocale" },
                }
            };
        }

        if (cultureInfo.ToString() == "fr-FR")
        {
            return new ExternalLocalizationResult()
            {
                Success = true,
                Localizations = new Dictionary<string, string>()
                {
                    { "CurrentLocaleName", "S3French" },
                    { "LocalizationCurrentProviderIs", "S3Le fournisseur de localisation actuel est:" },
                    { "LocalizationProviderName", "S3Json" },
                    { "MocaleDescription", "S3Framework de localisation pour .NET Maui" },
                    { "MocaleTitle", "S3Mocale" },
                }
            };
        }

        return new ExternalLocalizationResult()
        {
            Success = false,
            Localizations = [],
        };
    }
}

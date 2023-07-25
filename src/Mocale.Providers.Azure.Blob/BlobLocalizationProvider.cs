using System.Globalization;
namespace Mocale.Providers.Azure.Blob;

internal sealed class BlobLocalizationProvider : IExternalLocalizationProvider
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IExternalLocalizationResult> GetValuesForCultureAsync(CultureInfo cultureInfo)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        if (cultureInfo.ToString() == "en-GB")
        {
            return new ExternalLocalizationResult()
            {
                Success = true,
                Localizations = new Dictionary<string, string>()
                {
                    { "CurrentLocaleName", "AzEnglish" },
                    { "LocalizationCurrentProviderIs", "AzThe current localization provider is:" },
                    { "LocalizationProviderName", "AzJson" },
                    { "MocaleDescription", "AzLocalization framework for .NET Maui" },
                    { "MocaleTitle", "AzMocale" },
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
                    { "CurrentLocaleName", "AzFrench" },
                    { "LocalizationCurrentProviderIs", "AzLe fournisseur de localisation actuel est:" },
                    { "LocalizationProviderName", "AzJson" },
                    { "MocaleDescription", "AzFramework de localisation pour .NET Maui" },
                    { "MocaleTitle", "AzMocale" },
                }
            };
        }

        return new ExternalLocalizationResult()
        {
            Success = false,
            Localizations = new Dictionary<string, string>(),
        };
    }
}

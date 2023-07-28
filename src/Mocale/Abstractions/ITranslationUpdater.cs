using System.Globalization;
namespace Mocale.Abstractions;

public interface ITranslationUpdater
{
    void UpdateTranslations(CultureInfo cultureInfo, Dictionary<string, string> translations, TranslationSource source);
}

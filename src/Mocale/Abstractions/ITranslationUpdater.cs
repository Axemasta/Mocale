namespace Mocale.Abstractions;

public interface ITranslationUpdater
{
    void UpdateTranslations(Localization localization, TranslationSource source);
}

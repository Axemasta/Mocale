namespace Mocale.Abstractions;

/// <summary>
/// Translation Updater
/// </summary>
public interface ITranslationUpdater
{
    /// <summary>
    /// Update Translations From Source
    /// </summary>
    /// <param name="localization"></param>
    /// <param name="source"></param>
    void UpdateTranslations(Localization localization, TranslationSource source);
}

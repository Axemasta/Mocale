using System.ComponentModel;
using System.Globalization;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Models;
using Mocale.Testing.Extensions;

namespace Mocale.Testing;

/// <summary>
/// Translator Manager Proxy class to enable unit testing of views that use Mocale translations.
/// </summary>
public class TranslatorManagerProxy : IInternalTranslatorManager, INotifyPropertyChanged
{
    internal Dictionary<string, string> PreferredLocalizations { get; set; } = [];

    internal Dictionary<string, string> BackupLocalizations { get; set; } = [];

    /// <inheritdoc/>
    public CultureInfo? CurrentCulture { get; private set; }

    /// <summary>
    /// Used internally to provide translations
    /// </summary>
    /// <param name="resourceKey"></param>
    public object? this[string resourceKey] => Translate(resourceKey);

    /// <inheritdoc/>
    public string Translate(string key)
    {
        if (PreferredLocalizations.TryGetValue(key, out var externalTranslation))
        {
            return externalTranslation;
        }

        if (BackupLocalizations.TryGetValue(key, out var internalTranslation))
        {
            return internalTranslation;
        }

        return "$" + key + "$";
    }

    /// <inheritdoc/>
    public string Translate(string key, object[] parameters)
    {
        var translation = Translate(key);

        if (string.IsNullOrEmpty(translation))
        {
            return translation;
        }

        try
        {
            return string.Format(CurrentCulture, translation, parameters);
        }
        catch
        {
            return translation;
        }
    }

    /// <inheritdoc/>
    public void UpdateTranslations(Localization localization, TranslationSource source, bool notify = true)
    {
        if (!Equals(CurrentCulture, localization.CultureInfo))
        {
            CurrentCulture = localization.CultureInfo;
            PreferredLocalizations.Clear();
            BackupLocalizations.Clear();
        }

        switch (source)
        {
            case TranslationSource.External:
            case TranslationSource.WarmCache:
            case TranslationSource.ColdCache:
            {
                PreferredLocalizations.AddOrUpdateValues(localization.Translations);
                break;
            }

            case TranslationSource.Internal:
            {
                BackupLocalizations.AddOrUpdateValues(localization.Translations);
                break;
            }

            default:
                break;
        }

        if (notify)
        {
            RaisePropertyChanged();
        }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public void RaisePropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

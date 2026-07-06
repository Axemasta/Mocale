using System.ComponentModel;
using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Managers;

/// <inheritdoc />
internal partial class TranslatorManager : IInternalTranslatorManager
{
    #region Fields

    private readonly ILogger logger;
    private readonly IMocaleConfiguration mocaleConfiguration;

    #endregion Fields

    #region Properties

    public CultureInfo? CurrentCulture { get; private set; }

    internal Dictionary<string, string> PreferredLocalizations { get; } = [];

    internal Dictionary<string, string> BackupLocalizations { get; } = [];

    #endregion Properties

    #region Constructors

    public TranslatorManager(
        ILogger<TranslatorManager> logger,
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager)
    {
        this.logger = Guard.Against.Null(logger);

        mocaleConfigurationManager = Guard.Against.Null(mocaleConfigurationManager);
        mocaleConfiguration = mocaleConfigurationManager.Configuration;
    }

    #endregion Constructors

    #region Methods

    public object this[string resourceKey] => Translate(resourceKey);

    #endregion Methods

    #region Interface Implementations

    #region - ITranslatorManager

    public string Translate(string key)
    {
        if (PreferredLocalizations.TryGetValue(key, out var externalTranslation))
        {
            return externalTranslation;
        }

        if (BackupLocalizations.TryGetValue(key, out var internalTranslation))
        {
            LogKeyKeyWasFoundInBackupLocalizations(key);
            return internalTranslation;
        }

        LogResourceKeyNotFoundResourceKey(key);

        if (!mocaleConfiguration.ShowMissingKeys)
        {
            return string.Empty;
        }

        return mocaleConfiguration.NotFoundSymbol + key + StringExtension.Reverse(mocaleConfiguration.NotFoundSymbol);
    }

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
        catch (Exception ex)
        {
            LogAnExceptionOccurredFormatingTranslationForKeyKeyTranslationWithParametersParameters(ex, key, translation, parameters);
            return translation;
        }
    }

    #endregion - ITranslatorManager

    #region - ITranslationUpdater

    /// <inheritdoc />
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
            default:
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
        }

        if (notify)
        {
            RaisePropertyChanged();
        }
    }

    #endregion - ITranslationUpdater

    #region - INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged(string? propertyName = null)
    {
        if (PropertyChanged is null)
        {
            return;
        }

        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion - INotifyPropertyChanged

    #endregion Interface Implementations

    #region Logging

    [LoggerMessage(LogLevel.Debug, "Key: {Key} was found in backup localizations")]
    partial void LogKeyKeyWasFoundInBackupLocalizations(string key);

    [LoggerMessage(LogLevel.Warning, "Resource key not found '{ResourceKey}'")]
    partial void LogResourceKeyNotFoundResourceKey(string resourceKey);

    [LoggerMessage(LogLevel.Error, "An exception occurred formating translation for key {Key}: '{Translation}' with parameters: {Parameters}")]
    partial void LogAnExceptionOccurredFormatingTranslationForKeyKeyTranslationWithParametersParameters(Exception exception, string key, string translation, object[] parameters);

    #endregion Logging
}

using System.ComponentModel;
using System.Globalization;
using Ardalis.GuardClauses;
namespace Mocale.Managers;

public class TranslatorManager : ITranslatorManager, ITranslationUpdater, INotifyPropertyChanged
{
    #region Fields

    private readonly ILogger logger;
    private readonly IMocaleConfiguration mocaleConfiguration;

    #endregion Fields

    #region Properties

    public CultureInfo? CurrentCulture { get; private set; }

    private Dictionary<string, string> ExternalLocalizations { get; set; } = new Dictionary<string, string>();

    private Dictionary<string, string> InternalLocalizations { get; set; } = new Dictionary<string, string>();

    #endregion Properties

    #region Constructors

    public TranslatorManager(
        ILogger<TranslatorManager> logger,
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager)
    {
        this.logger = Guard.Against.Null(logger, nameof(logger));

        mocaleConfigurationManager = Guard.Against.Null(mocaleConfigurationManager, nameof(mocaleConfigurationManager));
        mocaleConfiguration = mocaleConfigurationManager.Configuration;
    }

    #endregion Constructors

    #region Methods

    public object? this[string resourceKey] => Translate(resourceKey);

    #endregion Methods

    #region Interface Implementations

    #region - ITranslatorManager

    public string? Translate(string key)
    {
        if (ExternalLocalizations.TryGetValue(key, out var externalTranslation))
        {
            return externalTranslation;
        }

        if (InternalLocalizations.TryGetValue(key, out var internalTranslation))
        {
            logger.LogDebug("Key: {Key} was found in Internal Localizations", key);
            return internalTranslation;
        }

        logger.LogWarning("Resource key not found '{ResourceKey}'", key);

        if (!mocaleConfiguration.ShowMissingKeys)
        {
            return string.Empty;
        }

        return mocaleConfiguration.NotFoundSymbol + key + mocaleConfiguration.NotFoundSymbol;
    }

    #endregion - ITranslatorManager

    #region - ITranslationUpdater

    public void UpdateTranslations(CultureInfo cultureInfo, Dictionary<string, string> translations, TranslationSource source)
    {
        CurrentCulture = cultureInfo;

        switch (source)
        {
            case TranslationSource.External:
            {
                ExternalLocalizations.Clear();
                ExternalLocalizations = translations;
                break;
            }

            default:
            {
                InternalLocalizations.Clear();
                InternalLocalizations = translations;
                break;
            }
        }

        RaisePropertyChanged();
    }

    #endregion - ITranslationUpdater

    #region - INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void RaisePropertyChanged(string? propertyName = null)
    {
        if (PropertyChanged is null)
        {
            return;
        }

        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion - INotifyPropertyChanged

    #endregion Interface Implementations
}

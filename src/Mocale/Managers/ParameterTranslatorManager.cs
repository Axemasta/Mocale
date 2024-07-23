using System.ComponentModel;
using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Managers;

internal class ParameterTranslatorManager : IParameterTranslatorManager, INotifyPropertyChanged
{
    private readonly ITranslatorManager translatorManager;

    private object[]? parameters;

    public CultureInfo? CurrentCulture => throw new NotImplementedException();

    public ParameterTranslatorManager(ITranslatorManager translatorManager)
    {
        this.translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

        // TODO: Make a centralized way of updating translations!
        if (translatorManager is TranslatorManager tMan)
        {
            tMan.PropertyChanged += (s, e) => RaisePropertyChanged(null);
        }
    }

    public void SetParameters(object[] parameters)
    {
        this.parameters = parameters;
    }

    public object? this[string resourceKey] => Translate(resourceKey);

    public string? Translate(string resourceKey)
    {
        if (parameters is null)
        {
            throw new ArgumentException("Please call SetParameters(object[] ..) before ");
        }

        return translatorManager.Translate(resourceKey, parameters);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void RaisePropertyChanged(string? propertyName = null)
    {
        if (PropertyChanged is null)
        {
            return;
        }

        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string? Translate(string key, object[] parameters)
    {
        return translatorManager.Translate(key, parameters);
    }
}


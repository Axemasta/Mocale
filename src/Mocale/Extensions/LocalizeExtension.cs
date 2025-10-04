using System.ComponentModel;
using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
///     Localize Markup Extension.
///     Used to localize a given translation key.
/// </summary>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Key))]
public class LocalizeExtension(ITranslatorManager translatorManager) : LocalizeBindingExtensionBase(translatorManager)
{
    /// <summary>
    ///     Localize Extension
    /// </summary>
    public LocalizeExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <summary>
    ///     The translation key
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    ///     Converter
    /// </summary>
    public IValueConverter? Converter { get; set; }

    private string TranslatedValue => translatorManager[Key!];

    /// <inheritdoc />
    public override BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        Guard.Against.NullOrEmpty(Key, nameof(Key));

        return BindingBase.Create<LocalizeExtension, string>(
            static source => source.TranslatedValue,
            mode: BindingMode.OneWay,
            converter: Converter,
            source: this);
    }

    /// <inheritdoc />
    protected override void OnTranslatorManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TranslatedValue));
    }
}

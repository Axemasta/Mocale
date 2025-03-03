using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
///     Localize Markup Extension.
///     Used to localize a given translation key.
/// </summary>
/// <param name="translatorManager">Translator Manager</param>
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

    /// <inheritdoc />
    public override Binding ProvideValue(IServiceProvider serviceProvider)
    {
        Guard.Against.NullOrEmpty(Key, nameof(Key));

        return new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Key}]",
            Source = translatorManager,
            Converter = Converter
        };
    }
}

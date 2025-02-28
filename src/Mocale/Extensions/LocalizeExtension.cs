using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
///     Localize Markup Extension.
///     Used to localize a given translation key.
/// </summary>
/// <param name="translatorManager">Translator Manager</param>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Key))]
public class LocalizeExtension(ITranslatorManager translatorManager) : IMarkupExtension<BindingBase>
{
    private readonly ITranslatorManager translatorManager =
        Guard.Against.Null(translatorManager, nameof(translatorManager));

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
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
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

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }

    internal ITranslatorManager GetTranslatorManager()
    {
        return translatorManager;
    }
}

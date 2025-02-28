using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
/// Localize Binding Markup Extension.
/// Used to localize using a format string with a single binded parameter
/// </summary>
/// <param name="translatorManager">Translator Manager</param>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Path))]
public class LocalizeBindingExtension(ITranslatorManager translatorManager) : IMarkupExtension, IMultiValueConverter
{
    private readonly ITranslatorManager translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

    /// <summary>
    /// The translation key
    /// </summary>
    public string? TranslationKey { get; set; }

    /// <summary>
    /// Translation parameters
    /// </summary>
    public string? Parameters { get; set; }

    /// <inheritdoc/>
    public string Path { get; set; } = ".";

    /// <inheritdoc/>
    public BindingMode Mode { get; set; } = BindingMode.OneWay;

    /// <inheritdoc/>
    public string StringFormat { get; set; } = "{0}";

    /// <inheritdoc/>
    public IValueConverter? Converter { get; set; }

    /// <inheritdoc/>
    public object? ConverterParameter { get; set; }

    /// <inheritdoc/>
    public object? Source { get; set; }

    /// <summary>
    /// Localize Extension
    /// </summary>
    public LocalizeBindingExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <inheritdoc/>
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return new MultiBinding()
        {
            StringFormat = StringFormat,
            Converter = this,
            Mode = BindingMode.OneWay,
            Bindings =
            [
                new Binding($"[{TranslationKey}]", BindingMode.OneWay, source: translatorManager),
                new Binding(Path, Mode, Converter, ConverterParameter, source: Source),
            ]
        };
    }

    /// <inheritdoc/>
    public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length != 2)
        {
            return string.Empty;
        }

        if (values[0] is not string localizedFormat)
        {
            return string.Empty;
        }

        // if (values[1] is not string localizeParameter)
        // {
        //     return string.Empty;
        // }

        return string.Format(translatorManager.CurrentCulture, localizedFormat, values[1]);
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

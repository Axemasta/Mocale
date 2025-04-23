using System.Globalization;

namespace Mocale.Extensions;

/// <summary>
/// Localize Binding Markup Extension.
/// Used to localize using a format string with a single binded parameter
/// </summary>
/// <param name="translatorManager">Translator Manager</param>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Path))]
public class LocalizeBindingExtension(ITranslatorManager translatorManager)
    : LocalizeMultiBindingExtensionBase(translatorManager), IMultiValueConverter
{
    /// <summary>
    /// Localize Extension
    /// </summary>
    public LocalizeBindingExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <summary>
    /// The translation key
    /// </summary>
    public string? TranslationKey { get; set; }

    /// <summary>
    /// Binding Path
    /// </summary>
    public string Path { get; set; } = ".";

    /// <summary>
    /// Binding Mode
    /// </summary>
    public BindingMode Mode { get; set; } = BindingMode.OneWay;

    /// <summary>
    /// String Format
    /// </summary>
    public string StringFormat { get; set; } = "{0}";

    /// <summary>
    /// Converter
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Converter Parameter
    /// </summary>
    public object? ConverterParameter { get; set; }

    /// <summary>
    /// The translation key converter to be applied before localization
    /// </summary>
    public IKeyConverter? KeyConverter { get; set; }

    /// <summary>
    /// The translation key converter parameter
    /// </summary>
    public object? KeyConverterParameter { get; set; }

    /// <summary>
    /// Source object for the binding
    /// </summary>
    public object? Source { get; set; }

    /// <inheritdoc />
    public object? Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(TranslationKey) && KeyConverter is not null)
        {
            return ConvertKeyConverterBinding(values, targetType, culture);
        }

        return ConvertTranslationKeyBinding(values);
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override MultiBinding ProvideValue(IServiceProvider serviceProvider)
    {
        if (!string.IsNullOrEmpty(TranslationKey))
        {
            return new MultiBinding
            {
                StringFormat = StringFormat,
                Converter = this,
                Mode = BindingMode.OneWay,
                Bindings =
                [
                    new Binding($"[{TranslationKey}]", BindingMode.OneWay, source: translatorManager),
                    new Binding(Path, Mode, Converter, ConverterParameter, source: Source)
                ]
            };
        }

        if (KeyConverter is not null)
        {
            return new MultiBinding
            {
                StringFormat = StringFormat,
                Converter = this,
                Mode = BindingMode.OneWay,
                Bindings =
                [
                    new Binding(nameof(ITranslatorManager.CurrentCulture), BindingMode.OneWay, source: translatorManager),
                    new Binding(Path, Mode, source: Source)
                ]
            };
        }

        throw new InvalidOperationException(
            $"Neither {nameof(TranslationKey)} or {nameof(KeyConverter)} were set. Use must set one of these, please see the documentation for details");
    }

    private string ConvertTranslationKeyBinding(object[]? values)
    {
        // values[0] will be translated value
        // values[1] will be the binded value
        if (values is null || values.Length != 2)
        {
            return string.Empty;
        }

        if (values[0] is not string localizedFormat)
        {
            return string.Empty;
        }

        string? formatParameter;

        if (values[1] is string localizeParameter)
        {
            formatParameter = localizeParameter;
        }
        else
        {
            // We need to ToString() here otherwise the formatting can go a bit wierd on other cultures...
            // TODO: This might need some future consideration ie hungarian would use decimal commas 1.000.000,01 and comma decimals!
            formatParameter = values[1]?.ToString();
        }

        return string.Format(translatorManager.CurrentCulture, localizedFormat, formatParameter);
    }

    internal string? ConvertKeyConverterBinding(object[]? values, Type targetType, CultureInfo culture)
    {
        if (KeyConverter is null)
        {
            throw new InvalidOperationException("Key converter was null, unable to convert binding.");
        }

        // values[0] will be the current culture
        // values[1] will be the binded value
        if (values is null || values.Length != 2)
        {
            return string.Empty;
        }

        var translationKey = KeyConverter.Convert(values[1], targetType, KeyConverterParameter, culture);

        if (string.IsNullOrEmpty(translationKey))
        {
            return string.Empty;
        }

        var translation = translatorManager.Translate(translationKey);

        if (Converter is null)
        {
            return translation;
        }

        return Converter.Convert(translation, targetType, ConverterParameter, culture) as string;
    }
}

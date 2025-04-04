using System.Globalization;

namespace Mocale.Extensions;

public class LocalizeEnumValueExtension(ITranslatorManager translatorManager) : LocalizeMultiBindingExtensionBase(translatorManager), IMultiValueConverter
{
    public LocalizeEnumValueExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <summary>
    /// Path for the binding
    /// </summary>
    public string Path { get; set; } = ".";

    /// <summary>
    /// Converter
    /// </summary>
    public IValueConverter? Converter { get; set; }

    public object? ConverterParameter { get; set; }

    /// <summary>
    /// Source of the binding
    /// </summary>
    public Enum? Source { get; set; }

    /// <summary>
    /// String format for the binding
    /// </summary>
    public string StringFormat { get; set; } = "{0}";

    public override MultiBinding ProvideValue(IServiceProvider serviceProvider)
    {
        return new MultiBinding()
        {
            StringFormat = StringFormat,
            Converter = this,
            Mode = BindingMode.OneWay,
            Bindings =
            [
                new Binding(nameof(translatorManager.CurrentCulture), BindingMode.OneWay, source: translatorManager),
                new Binding(Path, BindingMode.OneWay, source: Source)
            ]
        };
    }

    /// <inheritdoc />
    public object? Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length != 2)
        {
            return string.Empty;
        }

        if (values[1] is null)
        {
            return string.Empty;
        }

        if (values[1] is not Enum enumValue)
        {
            throw new NotSupportedException(
                $"Value must be of type {nameof(Enum)}, instead value was of type {values[1].GetType().Name}. Use LocalizeBinding to localize non enum values!");
        }
        var translation = translatorManager.TranslateEnum(enumValue);

        if (Converter is not null)
        {
            return Converter.Convert(translation, targetType, ConverterParameter, culture);
        }

        return translation;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

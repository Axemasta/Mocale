using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
/// Localize Enum Markup Extension
/// </summary>
/// <param name="translatorManager"></param>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(EnumBinding))]
public partial class LocalizeEnumExtension(ITranslatorManager translatorManager)
    : LocalizeMultiBindingExtensionBase(translatorManager), IMultiValueConverter
{
    /// <summary>
    /// The binding
    /// </summary>
    public BindingBase? EnumBinding { get; set; }

    /// <summary>
    /// Converter for the binding
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Converter parameter for the binding
    /// </summary>
    public object? ConverterParameter { get; set; }

    /// <summary>
    /// String format for the binding
    /// </summary>
    public string StringFormat { get; set; } = "{0}";

    /// <summary>
    /// Localize Extension
    /// </summary>
    public LocalizeEnumExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <inheritdoc/>
    public override MultiBinding ProvideValue(IServiceProvider serviceProvider)
    {
        Guard.Against.Null(EnumBinding, nameof(EnumBinding));

        return new MultiBinding()
        {
            StringFormat = StringFormat,
            Converter = this,
            Mode = BindingMode.OneWay,
            Bindings =
            [
                BindingBase.Create<ITranslatorManager, CultureInfo?>(
                    static source => source.CurrentCulture,
                    mode: BindingMode.OneWay,
                    source: translatorManager),
                EnumBinding
            ]
        };
    }

    /// <inheritdoc/>
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

        return Converter is not null ?
            Converter.Convert(translation, targetType, ConverterParameter, culture)
            : translation;
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

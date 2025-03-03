using System.ComponentModel;
using System.Globalization;
using Ardalis.GuardClauses;
using Mocale.Helper;

namespace Mocale.Extensions;

/// <summary>
/// Localize Enum Markup Extension
/// </summary>
/// <param name="mocaleConfiguration"></param>
/// <param name="translatorManager"></param>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Path))]
public class LocalizeEnumExtension(IMocaleConfiguration mocaleConfiguration, ITranslatorManager translatorManager)
    : LocalizeMultiBindingExtensionBase(translatorManager), IMultiValueConverter
{
    private readonly EnumTranslationKeyHelper enumTranslationKeyHelper = new(mocaleConfiguration);
    private readonly IMocaleConfiguration mocaleConfiguration = Guard.Against.Null(mocaleConfiguration, nameof(mocaleConfiguration));

    /// <summary>
    /// Path for the binding
    /// </summary>
    public string Path { get; set; } = ".";

    /// <summary>
    /// Binding mode for the binding
    /// </summary>
    public BindingMode Mode { get; set; } = BindingMode.OneWay;

    /// <summary>
    /// String format for the binding
    /// </summary>
    public string StringFormat { get; set; } = "{0}";

    /// <summary>
    /// Converter for the binding
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Converter parameter for the binding
    /// </summary>
    public object? ConverterParameter { get; set; }

    /// <summary>
    /// Source of the binding
    /// </summary>
    public object? Source { get; set; }

    /// <summary>
    /// Localize Extension
    /// </summary>
    public LocalizeEnumExtension()
        : this(MocaleLocator.MocaleConfiguration, MocaleLocator.TranslatorManager)
    {
    }

    /// <inheritdoc/>
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
                new Binding(Path, Mode, Converter, ConverterParameter, source: Source),
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
            throw new NotSupportedException($"Value must be of type {nameof(Enum)}, instead value was of type {values[1].GetType().Name}. Use LocalizeBinding to localize non enum values!");
        }

        if (!mocaleConfiguration.EnumBehavior.UseAttribute && mocaleConfiguration.EnumBehavior.OverrideRules.All(r => r.Key != enumValue.GetType()))
        {
            return enumValue.ToString();
        }

        var translationKey = enumTranslationKeyHelper.GetTranslationKey(enumValue);

        return translatorManager.Translate(translationKey);
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal IMocaleConfiguration GetMocaleConfiguration()
    {
        // Testing method
        return mocaleConfiguration;
    }
}

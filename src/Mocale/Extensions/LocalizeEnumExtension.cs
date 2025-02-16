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
public class LocalizeEnumExtension(IMocaleConfiguration mocaleConfiguration, ITranslatorManager translatorManager) : IMarkupExtension, IMultiValueConverter
{
    private readonly EnumTranslationKeyHelper enumTranslationKeyHelper = new(mocaleConfiguration);
    private readonly ITranslatorManager translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

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
    public LocalizeEnumExtension()
        : this(MocaleLocator.MocaleConfiguration, MocaleLocator.TranslatorManager)
    {
    }

    public object ProvideValue(IServiceProvider serviceProvider)
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

        if (values[0] is null)
        {
            return string.Empty;
        }

        if (values[1] is not Enum enumValue)
        {
            return string.Empty;
        }

        var translationKey = enumTranslationKeyHelper.GetTranslationKey(enumValue);

        return translatorManager.Translate(translationKey);
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

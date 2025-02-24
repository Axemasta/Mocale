using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
/// Localize Multi Binding Markup Extension.
/// Used to localize using a format string with multiple binded parameters
/// </summary>
/// <param name="translatorManager">Translator manager</param>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Bindings))]
public class LocalizeMultiBindingExtension(ITranslatorManager translatorManager) : IMarkupExtension, IMultiValueConverter
{
    private readonly ITranslatorManager translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

    /// <summary>
    /// The bindings to use as format parameters
    /// </summary>
    public IList<BindingBase> Bindings { get; set; } = [];

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
    public LocalizeMultiBindingExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <inheritdoc/>
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var bindings = new List<BindingBase>()
        {
            new Binding($"[{TranslationKey}]", BindingMode.OneWay, source: translatorManager),
        };

        bindings.AddRange(Bindings);

        return new MultiBinding()
        {
            StringFormat = StringFormat,
            Converter = this,
            Mode = BindingMode.OneWay,
            Bindings = bindings
        };
    }

    /// <inheritdoc/>
    public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length != Bindings.Count + 1)
        {
            return string.Empty;
        }

        if (values[0] is not string localizedFormat)
        {
            return string.Empty;
        }

        var arguments = values.Skip(1).ToArray();

        return string.Format(translatorManager.CurrentCulture, localizedFormat, arguments);
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

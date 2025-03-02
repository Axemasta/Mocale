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
public class LocalizeMultiBindingExtension(ITranslatorManager translatorManager)
    : LocalizeMultiBindingExtensionBase(translatorManager), IMultiValueConverter
{
    /// <summary>
    /// The bindings to use as format parameters
    /// </summary>
    public IList<BindingBase> Bindings { get; set; } = [];

    /// <summary>
    /// The translation key
    /// </summary>
    public string? TranslationKey { get; set; }

    /// <summary>
    /// String format for the multibinding
    /// </summary>
    public string StringFormat { get; set; } = "{0}";

    /// <summary>
    /// Localize Extension
    /// </summary>
    public LocalizeMultiBindingExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <inheritdoc/>
    public override MultiBinding ProvideValue(IServiceProvider serviceProvider)
    {
        Guard.Against.NullOrEmpty(TranslationKey, nameof(TranslationKey));
        Guard.Against.NullOrEmpty(Bindings, nameof(Bindings));

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

        if (values[0] is null)
        {
            return string.Empty;
        }

        if (values[0] is not string localizedFormat)
        {
            throw new InvalidOperationException($"The first value was not a string, actual type: {values[0].GetType().Name}, if this method has been automatically called by Mocale please raise an issue on GitHub!");
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

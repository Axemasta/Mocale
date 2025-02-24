using System.Globalization;
using Ardalis.GuardClauses;
using Mocale.Managers;
namespace Mocale.Extensions;

/// <summary>
/// Localize Markup Extension.
/// Used to localize a given translation key.
/// </summary>
/// <param name="translatorManager">Translator Manager</param>
[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Key))]
public class LocalizeExtension(ITranslatorManager translatorManager) : IMarkupExtension<BindingBase>
{
    private readonly ITranslatorManager translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

    /// <summary>
    /// The translation key
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Translation parameters
    /// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// Binding
    /// </summary>
    public BindingBase? Binding { get; set; }

    /// <summary>
    /// Parameter delimiter (for string)
    /// </summary>
    public char SplitDelimiter { get; set; } = '|';

    /// <summary>
    /// Converter
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Localize Extension
    /// </summary>
    public LocalizeExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    /// <inheritdoc/>
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        //TODO: Pass in itranslator
        if (!string.IsNullOrEmpty(Parameters))
        {
            var parameters = Parameters.Contains(SplitDelimiter)
                ? Parameters.Split(SplitDelimiter)
                : [Parameters];

            var parameterTranslatorManager = new ParameterTranslatorManager(translatorManager);
            parameterTranslatorManager.SetParameters(parameters);

            return new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Key}]",
                Source = parameterTranslatorManager,
                Converter = Converter,
            };
        }
        else if (Binding is Binding originalBinding)
        {
            var parameter = translatorManager.Translate(Key ?? string.Empty);

            var converter = new LocalizeBindingConverter();

            return new Binding()
            {
                Source = originalBinding.Source,
                Path = originalBinding.Path,
                Converter = converter,
                ConverterParameter = parameter,
                FallbackValue = originalBinding.FallbackValue,
                Mode = originalBinding.Mode,
                StringFormat = originalBinding.StringFormat,
                TargetNullValue = originalBinding.TargetNullValue,
            };
        }
        else
        {
            return new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Key}]",
                Source = translatorManager,
                Converter = Converter,
            };
        }
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }

    private class LocalizeBindingConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is string translation)
            {
                return string.Format(CultureInfo.InvariantCulture, translation, value);
            }

            throw new NotSupportedException($"Parameter was not set, cannot localize binded value: {value}");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

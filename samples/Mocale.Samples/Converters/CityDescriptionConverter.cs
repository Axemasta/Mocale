using System.Globalization;

namespace Mocale.Samples.Converters;

internal sealed class CityDescriptionConverter : IValueConverter
{
    private readonly ITranslatorManager translatorManager = MocaleLocator.TranslatorManager;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        if (value is not string)
        {
            throw new InvalidOperationException($"Value must be of type {nameof(String)}");
        }

        var key = $"CityDescription_{value}";

        //return new Binding
        //{
        //    Mode = BindingMode.OneWay,
        //    Path = $"[{key}]",
        //    Source = translatorManager,
        //};

        return translatorManager.Translate(key);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

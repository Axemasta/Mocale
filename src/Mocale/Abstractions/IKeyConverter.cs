using System.Globalization;

namespace Mocale.Abstractions;

/// <summary>
/// Translation Key Converter
/// </summary>
public interface IKeyConverter
{
    /// <summary>
    /// Convert the current object into a translation key
    /// </summary>
    /// <param name="value">The value to convert into a translation key</param>
    /// <param name="targetType">The target type for the control the localization is being applied</param>
    /// <param name="parameter">The parameter to use for the conversion</param>
    /// <param name="culture">The culture for the conversion</param>
    /// <returns></returns>
    string? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);
}

using System.Globalization;
namespace Mocale.Abstractions;

public interface ILocalizationManager
{
    CultureInfo CurrentCulture { get; }

    Task<bool> Initialize();

    Task<bool> SetCultureAsync(CultureInfo culture);
}

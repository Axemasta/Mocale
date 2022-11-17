using System.Globalization;

namespace Mocale.Abstractions
{
    public interface ILocalizationManager
    {
        CultureInfo CurrentCulture { get; }
    }
}

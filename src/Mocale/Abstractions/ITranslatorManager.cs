using System.Globalization;
namespace Mocale.Abstractions;

public interface ITranslatorManager
{
    CultureInfo? CurrentCulture { get; }

    string? Translate(string key);
}

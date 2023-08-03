using System.Globalization;
namespace Mocale.Models;

public class Localization
{
    public required CultureInfo CultureInfo { get; set; }

    public Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();

    // This is to get around nullable
    public static Localization Invariant => new()
    {
        CultureInfo = new CultureInfo(""),
    };
}

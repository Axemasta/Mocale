using System.Globalization;
namespace Mocale.Models;

/// <summary>
/// Localization
/// </summary>
public class Localization
{
    /// <summary>
    /// Corresponding culture
    /// </summary>
    public required CultureInfo CultureInfo { get; set; }

    /// <summary>
    /// Translations
    /// </summary>
    public Dictionary<string, string> Translations { get; set; } = [];

    /// <summary>
    /// Blank localization
    /// </summary>
    public static Localization Invariant => new()
    {
        CultureInfo = new CultureInfo(""),
    };
}

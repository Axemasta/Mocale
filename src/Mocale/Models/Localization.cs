using System.Globalization;
namespace Mocale.Models;

/// <summary>
/// Localization
/// </summary>
public class Localization(CultureInfo cultureInfo)
{
    /// <summary>
    /// Corresponding culture
    /// </summary>
    public CultureInfo CultureInfo { get; } = cultureInfo;

    /// <summary>
    /// Translations
    /// </summary>
    public Dictionary<string, string> Translations { get; set; } = [];

    /// <summary>
    /// Blank localization
    /// </summary>
    public static Localization Invariant => new(CultureInfo.InvariantCulture);
}

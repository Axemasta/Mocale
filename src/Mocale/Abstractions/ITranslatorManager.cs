using System.Globalization;
namespace Mocale.Abstractions;

/// <summary>
/// Translator Manager
/// </summary>
public interface ITranslatorManager
{
    /// <summary>
    /// Current Culture
    /// </summary>
    CultureInfo? CurrentCulture { get; }

    /// <summary>
    /// Translate
    /// </summary>
    /// <param name="key">Key to translate</param>
    /// <returns>Translation result</returns>
    string? Translate(string key);

    /// <summary>
    /// Translate
    /// </summary>
    /// <param name="key">Key to translate</param>
    /// <param name="parameters">Parameters to translate</param>
    /// <returns>Translation result</returns>
    string? Translate(string key, object[] parameters);
}

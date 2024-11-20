using System.Globalization;

namespace Mocale.Abstractions;

/// <summary>
/// Current Culture Manager
/// </summary>
public interface ICurrentCultureManager
{
    /// <summary>
    /// Get Active Culture
    /// </summary>
    /// <returns></returns>
    CultureInfo GetActiveCulture();

    /// <summary>
    /// Set Active Culture
    /// </summary>
    /// <param name="cultureInfo"></param>
    void SetActiveCulture(CultureInfo cultureInfo);
}


using System.Globalization;

namespace Mocale.Abstractions;

public interface ICurrentCultureManager
{
    CultureInfo GetActiveCulture();

    void SetActiveCulture(CultureInfo cultureInfo);
}


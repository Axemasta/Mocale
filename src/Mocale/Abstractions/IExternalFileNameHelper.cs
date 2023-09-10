using System.Globalization;

namespace Mocale.Abstractions;

internal interface IExternalFileNameHelper
{
    string GetExpectedFileName(CultureInfo culture);
}

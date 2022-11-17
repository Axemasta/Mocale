using System.Globalization;
using Mocale.Enums;

namespace Mocale.Abstractions;

public interface IMocaleConfiguration
{
    LocalResourceType ResourceType { get; }

    CultureInfo DefaultCulture { get; }
}

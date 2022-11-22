using System.Globalization;
using Mocale.Enums;

namespace Mocale.Abstractions;

public interface IMocaleConfiguration
{
    LocaleResourceType ResourceType { get; }

    CultureInfo DefaultCulture { get; }

    bool ShowMissingKeys { get; }

    string NotFoundSymbol { get; }

    bool UseExternalProvider { get; }
}

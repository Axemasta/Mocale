using System.Globalization;
using Mocale.Abstractions;
using Mocale.Enums;

namespace Mocale.Models;

public class MocaleConfiguration : IMocaleConfiguration
{
    public LocalResourceType ResourceType { get; internal set; }

    public CultureInfo DefaultCulture { get; set; } = Thread.CurrentThread.CurrentCulture; // Should this check the device for its language setting?

    public bool ShowMissingKeys { get; set; } = true;

    public string NotFoundSymbol { get; set; } = "$";
}

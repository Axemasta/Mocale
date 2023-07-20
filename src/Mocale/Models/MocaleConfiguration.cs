using System.Globalization;
namespace Mocale.Models;

public class MocaleConfiguration : IMocaleConfiguration
{
    public LocaleResourceType ResourceType { get; internal set; }

    public CultureInfo DefaultCulture { get; set; } = Thread.CurrentThread.CurrentCulture; // Should this check the device for its language setting?

    public bool ShowMissingKeys { get; set; } = true;

    public string NotFoundSymbol { get; set; } = "$";

    public bool UseExternalProvider { get; set; } = true;
}

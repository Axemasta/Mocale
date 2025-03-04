using System.Globalization;
namespace Mocale.Models;

/// <inheritdoc/>
public class MocaleConfiguration : IMocaleConfiguration
{
    /// <inheritdoc/>
    public LocaleResourceType ResourceType { get; internal set; }

    /// <inheritdoc/>
    public CultureInfo DefaultCulture { get; set; } = Thread.CurrentThread.CurrentCulture; // Should this check the device for its language setting?

    /// <inheritdoc/>
    public bool ShowMissingKeys { get; set; } = true;

    /// <inheritdoc/>
    public string NotFoundSymbol { get; set; } = "$";

    /// <inheritdoc/>
    public bool UseExternalProvider { get; set; } = true;

    /// <inheritdoc/>
    public bool SaveCultureChanged { get; set; } = true;

    /// <inheritdoc/>
    public LocalizeEnumBehavior EnumBehavior { get; set; } = new();
}

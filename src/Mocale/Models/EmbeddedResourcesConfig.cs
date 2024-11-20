using System.Reflection;
namespace Mocale.Models;

/// <inheritdoc/>
public class EmbeddedResourcesConfig : IEmbeddedResourcesConfig
{
    /// <inheritdoc/>
    public string ResourcesPath { get; set; } = "Locales";

    /// <inheritdoc/>
    public Assembly? ResourcesAssembly { get; set; }

    /// <inheritdoc/>
    public bool UseResourceFolder { get; set; } = true;
}

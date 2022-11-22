using System.Reflection;

namespace Mocale.Abstractions;

internal interface IEmbeddedResourcesConfig
{
    /// <summary>
    /// Path to the resources directory
    /// </summary>
    string ResourcesPath { get; }

    /// <summary>
    /// Assembly that contains the resources
    /// </summary>
    Assembly ResourcesAssembly { get; }

    /// <summary>
    /// Whether the resources live inside the Maui Resources folder or relative to the assembly
    /// </summary>
    bool UseResourceFolder { get; }
}

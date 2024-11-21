namespace Mocale.Abstractions;

/// <summary>
/// Configuration For App Resources
/// </summary>
public interface IAppResourcesConfig
{
    /// <summary>
    /// The type for the app resources
    /// </summary>
    Type? AppResourcesType { get; }
}

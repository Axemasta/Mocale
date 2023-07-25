namespace Mocale.Wrappers;

/// <summary>
/// Implementation wrapper for <see cref="IDateTime"/>
/// For .NET 8 this should use TimeProvider
/// </summary>
internal class DateTimeWrapper : IDateTime
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;
}

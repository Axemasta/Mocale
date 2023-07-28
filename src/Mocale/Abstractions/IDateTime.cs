namespace Mocale.Abstractions;

/// <summary>
/// Wrapper for <see cref="DateTime"/>
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// Calls <see cref="DateTime"/>.<see cref="DateTime.UtcNow"/>
    /// </summary>
    DateTime UtcNow { get; }
}

namespace ForumGuard.Application.Abstractions;

/// <summary>
/// Provides the current UTC time, abstracted so application services remain deterministic and testable.
/// <para>Services must read timestamps through this abstraction rather than calling <see cref="DateTime.UtcNow"/> directly.</para>
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current UTC instant.
    /// </summary>
    DateTime UtcNow { get; }
}

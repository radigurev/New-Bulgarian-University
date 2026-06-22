using ForumGuard.Application.Abstractions;

namespace ForumGuard.Tests.Application.Fakes;

/// <summary>
/// Deterministic <see cref="IClock"/> test double returning a fixed UTC instant.
/// <para>Lets service tests assert exact timestamps without depending on the wall clock.</para>
/// </summary>
public sealed class FixedClock : IClock
{
    /// <summary>
    /// Initializes the clock with the instant it will always return.
    /// </summary>
    /// <param name="utcNow">The fixed UTC instant.</param>
    public FixedClock(DateTime utcNow)
    {
        UtcNow = utcNow;
    }

    /// <inheritdoc />
    public DateTime UtcNow { get; }
}

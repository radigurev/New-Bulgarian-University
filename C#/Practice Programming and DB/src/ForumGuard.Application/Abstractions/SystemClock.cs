namespace ForumGuard.Application.Abstractions;

/// <summary>
/// Default <see cref="IClock"/> implementation returning the system UTC time.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}

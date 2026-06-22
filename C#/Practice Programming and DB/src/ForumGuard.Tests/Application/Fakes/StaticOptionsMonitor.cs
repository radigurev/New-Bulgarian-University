using Microsoft.Extensions.Options;

namespace ForumGuard.Tests.Application.Fakes;

/// <summary>
/// Minimal <see cref="IOptionsMonitor{TOptions}"/> test double exposing a single fixed value.
/// <para>Used to supply <c>ModerationOptions</c> to handlers and services without a configured host.</para>
/// </summary>
/// <typeparam name="TOptions">The options type the monitor exposes.</typeparam>
public sealed class StaticOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
{
    /// <summary>
    /// Initializes the monitor with the value it will always return.
    /// </summary>
    /// <param name="value">The fixed options value.</param>
    public StaticOptionsMonitor(TOptions value)
    {
        CurrentValue = value;
    }

    /// <inheritdoc />
    public TOptions CurrentValue { get; }

    /// <inheritdoc />
    public TOptions Get(string? name) => CurrentValue;

    /// <inheritdoc />
    public IDisposable? OnChange(Action<TOptions, string?> listener) => null;
}

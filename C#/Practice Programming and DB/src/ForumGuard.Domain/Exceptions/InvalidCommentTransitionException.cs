using ForumGuard.Domain.Enums;

namespace ForumGuard.Domain.Exceptions;

/// <summary>
/// Thrown when a comment lifecycle transition is requested that is not permitted by the state machine.
/// <para>See <see cref="CommentStatus"/>.</para>
/// </summary>
public sealed class InvalidCommentTransitionException : Exception
{
    /// <summary>
    /// Initializes a new instance describing a rejected transition between two comment statuses.
    /// </summary>
    /// <param name="from">The current status the comment is in.</param>
    /// <param name="to">The target status that was requested.</param>
    public InvalidCommentTransitionException(CommentStatus from, CommentStatus to)
        : base($"Transition from '{from}' to '{to}' is not allowed.")
    {
        From = from;
        To = to;
    }

    /// <summary>
    /// Gets the current status the comment was in when the transition was rejected.
    /// </summary>
    public CommentStatus From { get; }

    /// <summary>
    /// Gets the target status that was requested and rejected.
    /// </summary>
    public CommentStatus To { get; }
}

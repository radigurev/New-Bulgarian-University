namespace ForumGuard.Domain.Entities;

/// <summary>
/// Represents a forum thread that owns a collection of comments.
/// <para>See <see cref="Comment"/>.</para>
/// </summary>
public sealed class ForumThread
{
    /// <summary>
    /// The maximum permitted length of <see cref="Title"/>.
    /// </summary>
    public const int MaxTitleLength = 200;

    private ForumThread()
    {
        Title = string.Empty;
    }

    /// <summary>
    /// Creates a new forum thread.
    /// </summary>
    /// <param name="title">The thread title; required, non-whitespace, length 1..200.</param>
    /// <param name="createdById">The identifier of the user who created the thread.</param>
    /// <param name="createdAtUtc">The UTC instant at which the thread was created.</param>
    public ForumThread(string title, Guid createdById, DateTime createdAtUtc)
    {
        ValidateTitle(title);

        Id = Guid.NewGuid();
        Title = title;
        CreatedById = createdById;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Gets the primary key of the thread.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the thread title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who created the thread.
    /// </summary>
    public Guid CreatedById { get; private set; }

    /// <summary>
    /// Gets the UTC instant at which the thread was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Thread title must be non-null and non-whitespace.", nameof(title));
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ArgumentException($"Thread title must not exceed {MaxTitleLength} characters.", nameof(title));
        }
    }
}

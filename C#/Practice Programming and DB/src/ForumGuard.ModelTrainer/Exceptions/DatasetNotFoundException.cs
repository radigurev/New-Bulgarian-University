namespace ForumGuard.ModelTrainer.Exceptions;

/// <summary>
/// Thrown when the configured training dataset path is missing/empty or points to an unreadable file.
/// <para>See SDD-FORUM-024 (E1). The trainer aborts with a non-zero exit code and produces no
/// <c>model.zip</c>.</para>
/// </summary>
public sealed class DatasetNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message describing the missing or unreadable dataset.</param>
    public DatasetNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetNotFoundException"/> class wrapping an inner cause.
    /// </summary>
    /// <param name="message">The message describing the missing or unreadable dataset.</param>
    /// <param name="innerException">The underlying IO failure.</param>
    public DatasetNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

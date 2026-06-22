namespace ForumGuard.ModelTrainer.Exceptions;

/// <summary>
/// Thrown when the trained model cannot be exported to the configured <c>model.zip</c> path because
/// the destination is not writable, the directory is missing, the file is locked, or the ML.NET save
/// operation fails.
/// <para>See SDD-FORUM-024 (E5). A failed export MUST NOT be reported as success and MUST NOT leave a
/// partial/corrupt artifact at the destination; the trainer exits non-zero.</para>
/// </summary>
public sealed class ModelExportException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelExportException"/> class.
    /// </summary>
    /// <param name="message">The message describing the export failure.</param>
    public ModelExportException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelExportException"/> class wrapping an inner cause.
    /// </summary>
    /// <param name="message">The message describing the export failure.</param>
    /// <param name="innerException">The underlying IO or ML.NET save failure.</param>
    public ModelExportException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

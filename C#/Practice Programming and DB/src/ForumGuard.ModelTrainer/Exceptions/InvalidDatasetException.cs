namespace ForumGuard.ModelTrainer.Exceptions;

/// <summary>
/// Thrown when the training dataset is readable but invalid: a single-class corpus, too few rows
/// overall or per class, a missing/wrong header, an unparseable row, or a label that is neither
/// <c>Clean</c> nor <c>Toxic</c>.
/// <para>See SDD-FORUM-024 (E2, E3, E4 and validation rules DV-1, DV-2, DV-3). The trainer aborts
/// before invoking the TextClassification trainer and produces no <c>model.zip</c>.</para>
/// </summary>
public sealed class InvalidDatasetException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDatasetException"/> class.
    /// </summary>
    /// <param name="message">The message describing the dataset validation failure.</param>
    public InvalidDatasetException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDatasetException"/> class wrapping an inner cause.
    /// </summary>
    /// <param name="message">The message describing the dataset validation failure.</param>
    /// <param name="innerException">The underlying parse failure.</param>
    public InvalidDatasetException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

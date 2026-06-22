namespace ForumGuard.ModelTrainer.Cli;

/// <summary>
/// Defines the process exit codes the trainer returns so a deployment script can detect failure.
/// <para>See SDD-FORUM-024 §2.6 rule 22 and the no-silent-success principle: any abort returns a
/// non-zero code and produces no <c>model.zip</c>.</para>
/// </summary>
public static class TrainerExitCode
{
    /// <summary>
    /// Training completed and a <c>model.zip</c> was exported.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// The command-line arguments were invalid.
    /// </summary>
    public const int InvalidArguments = 1;

    /// <summary>
    /// The dataset was missing or unreadable (E1).
    /// </summary>
    public const int DatasetNotFound = 2;

    /// <summary>
    /// The dataset failed validation (E2/E3/E4).
    /// </summary>
    public const int InvalidDataset = 3;

    /// <summary>
    /// The trained model could not be exported (E5).
    /// </summary>
    public const int ExportFailed = 4;

    /// <summary>
    /// Training was cancelled by the operator (E6).
    /// </summary>
    public const int Cancelled = 5;

    /// <summary>
    /// An unexpected failure occurred, most commonly a missing native <c>libtorch-cpu</c> backend.
    /// </summary>
    public const int UnexpectedFailure = 6;
}

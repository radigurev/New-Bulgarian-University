namespace ForumGuard.Application.Moderation;

/// <summary>
/// Carries the per-handler traversal outcome inside the pipeline: a short-circuiting verdict (if any) and the score observed.
/// <para>See <see cref="CommentModerationPipeline"/>.</para>
/// </summary>
/// <param name="Verdict">A flagged verdict when the handler short-circuited, otherwise <c>null</c>.</param>
/// <param name="ObservedScore">The toxic-score the handler reported.</param>
internal readonly record struct HandlerEvaluation(ModerationVerdict? Verdict, float ObservedScore);

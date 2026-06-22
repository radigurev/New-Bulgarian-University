namespace ForumGuard.Infrastructure.Analysis;

/// <summary>
/// Represents the input schema consumed by the NAS-BERT TextClassification model at inference time.
/// <para>See SDD-FORUM-020 and SDD-FORUM-024. This schema is shared with the offline trainer
/// (<c>ForumGuard.ModelTrainer</c>): the trained <c>model.zip</c> maps the <see cref="Text"/> column
/// onto the NAS-BERT <c>sentence1ColumnName</c>. The <see cref="ModelOutput"/> type is the matching
/// output schema loaded via <c>PredictionEnginePool&lt;ModelInput, ModelOutput&gt;</c>.</para>
/// </summary>
public sealed class ModelInput
{
    /// <summary>
    /// Gets or sets the raw comment text to be classified as <c>Clean</c> or <c>Toxic</c>.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the label column required by the saved training pipeline's
    /// <c>MapValueToKey("Label")</c> step so the prediction engine can be created from the saved model.
    /// It is not used at inference — the prediction is derived solely from <see cref="Text"/> — and
    /// defaults to a known training label (<c>"Clean"</c>) to avoid an unmapped-key edge case.
    /// </summary>
    public string Label { get; set; } = "Clean";
}

using Microsoft.ML.Data;

namespace ForumGuard.ModelTrainer.Dataset;

/// <summary>
/// Represents a single row of the seed training corpus loaded from the <c>Text,Label</c> CSV.
/// <para>See SDD-FORUM-024 §2.2. <see cref="Text"/> is column index <c>0</c>; <see cref="Label"/> is
/// column index <c>1</c> and MUST be exactly <c>"Clean"</c> or <c>"Toxic"</c>. This schema feeds the
/// TextClassification pipeline (<c>sentence1ColumnName = "Text"</c>, <c>labelColumnName = "Label"</c>).</para>
/// </summary>
public sealed class TrainingDataRow
{
    /// <summary>
    /// Gets or sets the comment text to classify; loaded from CSV column index <c>0</c>.
    /// </summary>
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the toxicity label; loaded from CSV column index <c>1</c>; one of <c>"Clean"</c> / <c>"Toxic"</c>.
    /// </summary>
    [LoadColumn(1)]
    public string Label { get; set; } = string.Empty;
}

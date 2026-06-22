using System.Text;
using ForumGuard.ModelTrainer.Dataset;
using ForumGuard.ModelTrainer.Exceptions;

namespace ForumGuard.Tests.ModelTrainer;

/// <summary>
/// Unit tests for <see cref="SeedDatasetLoader"/> covering the SDD-FORUM-024 seed-CSV contract: a valid
/// two-class load, the missing-file (E1), single-class (E2 / DV-1), too-few-rows (E3 / DV-2),
/// missing/wrong-header and bad-label (E4 / DV-3) failures, and RFC-4180 quoted-comma parsing.
/// <para>Each test writes a unique UTF-8 temporary CSV fixture and removes it in tear-down. No model
/// training or libtorch is involved; the loader is a pure unit under test.</para>
/// </summary>
[TestFixture]
[Category("SDD-FORUM-024")]
public sealed class SeedDatasetLoaderTests
{
    private readonly List<string> _temporaryFiles = [];

    [TearDown]
    public void TearDown()
    {
        foreach (string path in _temporaryFiles)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        _temporaryFiles.Clear();
    }

    [Test]
    public void Load_ValidTwoClassCsv_ReturnsAllRowsWithTextAndLabel()
    {
        // Arrange
        string path = WriteCsv(BuildBalancedCorpus(cleanRows: 6, toxicRows: 6));
        SeedDatasetLoader sut = new();

        // Act
        IReadOnlyList<TrainingDataRow> rows = sut.Load(path);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rows, Has.Count.EqualTo(12));
            Assert.That(rows.Count(row => row.Label == TrainingLabels.Clean), Is.EqualTo(6));
            Assert.That(rows.Count(row => row.Label == TrainingLabels.Toxic), Is.EqualTo(6));
            Assert.That(rows.All(row => !string.IsNullOrWhiteSpace(row.Text)), Is.True);
        });
    }

    [Test]
    public void Load_MissingDatasetFile_ThrowsDatasetNotFoundException()
    {
        // Arrange
        string missingPath = Path.Combine(Path.GetTempPath(), $"forumguard-missing-{Guid.NewGuid():N}.csv");
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(() => sut.Load(missingPath), Throws.TypeOf<DatasetNotFoundException>());
    }

    [Test]
    public void Load_EmptyDatasetPath_ThrowsDatasetNotFoundException()
    {
        // Arrange
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(() => sut.Load(string.Empty), Throws.TypeOf<DatasetNotFoundException>());
    }

    [Test]
    public void Load_SingleClassDataset_ThrowsInvalidDatasetExceptionSingleClass()
    {
        // Arrange
        List<string> lines = ["Text,Label"];
        for (int i = 0; i < 12; i++)
        {
            lines.Add($"clean comment number {i},Clean");
        }

        string path = WriteCsv(lines);
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("Single-class"));
    }

    [Test]
    public void Load_TooFewRowsOverall_ThrowsInvalidDatasetExceptionInsufficientData()
    {
        // Arrange
        string path = WriteCsv(BuildBalancedCorpus(cleanRows: 2, toxicRows: 2));
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("Insufficient training data"));
    }

    [Test]
    public void Load_TooFewRowsForOneClass_ThrowsInvalidDatasetExceptionInsufficientData()
    {
        // Arrange
        DatasetValidationOptions options = new(minimumTotalRows: 5, minimumRowsPerClass: 3);
        string path = WriteCsv(BuildBalancedCorpus(cleanRows: 5, toxicRows: 1));
        SeedDatasetLoader sut = new(options);

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("Insufficient training data"));
    }

    [Test]
    public void Load_MissingHeaderRow_ThrowsInvalidDatasetExceptionFormat()
    {
        // Arrange
        List<string> lines = ["a clean comment,Clean", "a toxic comment,Toxic"];
        string path = WriteCsv(lines);
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("header"));
    }

    [Test]
    public void Load_WrongHeaderColumns_ThrowsInvalidDatasetExceptionFormat()
    {
        // Arrange
        List<string> lines = ["Comment,Category", "a clean comment,Clean", "a toxic comment,Toxic"];
        string path = WriteCsv(lines);
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("Text,Label"));
    }

    [Test]
    public void Load_LabelNotCleanOrToxic_ThrowsInvalidDatasetExceptionFormat()
    {
        // Arrange
        List<string> lines = ["Text,Label", "a clean comment,Clean", "a strange comment,Spam"];
        string path = WriteCsv(lines);
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("Spam"));
    }

    [Test]
    public void Load_RowWithExtraColumn_ThrowsInvalidDatasetExceptionFormat()
    {
        // Arrange
        List<string> lines = ["Text,Label", "a clean comment,Clean", "unquoted, comma comment,Toxic"];
        string path = WriteCsv(lines);
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("2 fields"));
    }

    [Test]
    public void Load_QuotedTextContainingComma_ParsesAsSingleTextField()
    {
        // Arrange
        List<string> lines = ["Text,Label"];
        lines.Add("\"Well, that is a thoughtful, balanced point\",Clean");
        for (int i = 0; i < 5; i++)
        {
            lines.Add($"another clean remark {i},Clean");
        }

        for (int i = 0; i < 6; i++)
        {
            lines.Add($"a clearly toxic insult {i},Toxic");
        }

        string path = WriteCsv(lines);
        SeedDatasetLoader sut = new();

        // Act
        IReadOnlyList<TrainingDataRow> rows = sut.Load(path);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rows, Has.Count.EqualTo(12));
            Assert.That(rows[0].Text, Is.EqualTo("Well, that is a thoughtful, balanced point"));
            Assert.That(rows[0].Label, Is.EqualTo(TrainingLabels.Clean));
        });
    }

    [Test]
    public void Load_WhitespaceOnlyText_ThrowsInvalidDatasetExceptionFormat()
    {
        // Arrange
        List<string> lines = ["Text,Label", "\"   \",Clean", "a toxic comment,Toxic"];
        string path = WriteCsv(lines);
        SeedDatasetLoader sut = new();

        // Act & Assert
        Assert.That(
            () => sut.Load(path),
            Throws.TypeOf<InvalidDatasetException>().With.Message.Contains("non-whitespace"));
    }

    /// <summary>
    /// Builds a header plus the requested number of clean and toxic data rows.
    /// </summary>
    /// <param name="cleanRows">The number of <c>Clean</c> rows to generate.</param>
    /// <param name="toxicRows">The number of <c>Toxic</c> rows to generate.</param>
    /// <returns>The CSV lines including the header.</returns>
    private static List<string> BuildBalancedCorpus(int cleanRows, int toxicRows)
    {
        List<string> lines = ["Text,Label"];
        for (int i = 0; i < cleanRows; i++)
        {
            lines.Add($"a perfectly polite remark {i},Clean");
        }

        for (int i = 0; i < toxicRows; i++)
        {
            lines.Add($"a clearly toxic insult {i},Toxic");
        }

        return lines;
    }

    /// <summary>
    /// Writes the supplied lines to a unique UTF-8 temporary CSV file and tracks it for cleanup.
    /// </summary>
    /// <param name="lines">The CSV lines to write.</param>
    /// <returns>The path to the written file.</returns>
    private string WriteCsv(IEnumerable<string> lines)
    {
        string path = Path.Combine(Path.GetTempPath(), $"forumguard-seed-{Guid.NewGuid():N}.csv");
        File.WriteAllLines(path, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        _temporaryFiles.Add(path);
        return path;
    }
}

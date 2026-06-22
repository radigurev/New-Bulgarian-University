using System.Text;
using ForumGuard.ModelTrainer.Dataset;

namespace ForumGuard.ModelTrainer.Csv;

/// <summary>
/// A minimal RFC-4180 CSV reader that splits UTF-8 text into records of fields, honoring quoted
/// fields that contain commas, embedded quotes (escaped as <c>""</c>), and newlines.
/// <para>See SDD-FORUM-024 §2.2 rule 14 and DV-3. Used by <see cref="SeedDatasetLoader"/> so a comment
/// body containing a comma is read as one <c>Text</c> field rather than two columns.</para>
/// </summary>
public static class Rfc4180Reader
{
    /// <summary>
    /// Parses the supplied CSV content into a list of records, each a list of string fields.
    /// </summary>
    /// <param name="content">The full CSV text content.</param>
    /// <returns>The parsed records; an empty list when the content is empty.</returns>
    public static IReadOnlyList<IReadOnlyList<string>> Parse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        List<IReadOnlyList<string>> records = [];
        List<string> currentRecord = [];
        StringBuilder field = new();
        bool inQuotes = false;
        bool fieldStarted = false;

        for (int i = 0; i < content.Length; i++)
        {
            char character = content[i];

            if (inQuotes)
            {
                i = ConsumeQuotedCharacter(content, i, character, field, ref inQuotes);
                continue;
            }

            switch (character)
            {
                case '"':
                    inQuotes = true;
                    fieldStarted = true;
                    break;
                case ',':
                    EndField(currentRecord, field);
                    fieldStarted = true;
                    break;
                case '\r':
                    break;
                case '\n':
                    fieldStarted = EndRecord(records, currentRecord, field, fieldStarted);
                    currentRecord = [];
                    break;
                default:
                    field.Append(character);
                    fieldStarted = true;
                    break;
            }
        }

        FlushTrailingRecord(records, currentRecord, field, fieldStarted);
        return records;
    }

    private static int ConsumeQuotedCharacter(string content, int index, char character, StringBuilder field, ref bool inQuotes)
    {
        if (character != '"')
        {
            field.Append(character);
            return index;
        }

        bool isEscapedQuote = index + 1 < content.Length && content[index + 1] == '"';
        if (isEscapedQuote)
        {
            field.Append('"');
            return index + 1;
        }

        inQuotes = false;
        return index;
    }

    private static void EndField(List<string> currentRecord, StringBuilder field)
    {
        currentRecord.Add(field.ToString());
        field.Clear();
    }

    private static bool EndRecord(
        List<IReadOnlyList<string>> records,
        List<string> currentRecord,
        StringBuilder field,
        bool fieldStarted)
    {
        EndField(currentRecord, field);
        records.Add(currentRecord);
        return false;
    }

    private static void FlushTrailingRecord(
        List<IReadOnlyList<string>> records,
        List<string> currentRecord,
        StringBuilder field,
        bool fieldStarted)
    {
        if (fieldStarted || field.Length > 0 || currentRecord.Count > 0)
        {
            EndField(currentRecord, field);
            records.Add(currentRecord);
        }
    }
}

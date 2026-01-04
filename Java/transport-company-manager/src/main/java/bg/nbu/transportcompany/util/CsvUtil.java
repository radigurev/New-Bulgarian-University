package bg.nbu.transportcompany.util;

import bg.nbu.transportcompany.exception.FileStorageException;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;

public final class CsvUtil {

    private CsvUtil() {
    }

    public static void writeAll(Path file, List<String[]> rows) {
        try {
            Files.createDirectories(file.getParent());
            try (BufferedWriter writer = Files.newBufferedWriter(file, StandardCharsets.UTF_8)) {
                for (String[] row : rows) {
                    writer.write(escapeRow(row));
                    writer.newLine();
                }
            }
        } catch (IOException ex) {
            throw new FileStorageException("Failed to write CSV file: " + file, ex);
        }
    }

    public static List<String[]> readAll(Path file) {
        try (BufferedReader reader = Files.newBufferedReader(file, StandardCharsets.UTF_8)) {
            List<String[]> rows = new ArrayList<>();
            String line;
            while ((line = reader.readLine()) != null) {
                rows.add(parseLine(line));
            }
            return rows;
        } catch (IOException ex) {
            throw new FileStorageException("Failed to read CSV file: " + file, ex);
        }
    }

    private static String escapeRow(String[] row) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < row.length; i++) {
            if (i > 0) {
                sb.append(',');
            }
            sb.append(escapeCell(row[i]));
        }
        return sb.toString();
    }

    private static String escapeCell(String cell) {
        if (cell == null) {
            return "";
        }
        String c = cell.replace("\"", "\"\"");
        if (c.contains(",") || c.contains("\"") || c.contains("\n")) {
            return "\"" + c + "\"";
        }
        return c;
    }

    private static String[] parseLine(String line) {
        List<String> cells = new ArrayList<>();
        StringBuilder cell = new StringBuilder();
        boolean inQuotes = false;

        for (int i = 0; i < line.length(); i++) {
            char ch = line.charAt(i);

            if (inQuotes) {
                if (ch == '"') {
                    if (i + 1 < line.length() && line.charAt(i + 1) == '"') {
                        cell.append('"');
                        i++;
                    } else {
                        inQuotes = false;
                    }
                } else {
                    cell.append(ch);
                }
            } else {
                if (ch == ',') {
                    cells.add(cell.toString());
                    cell.setLength(0);
                } else if (ch == '"') {
                    inQuotes = true;
                } else {
                    cell.append(ch);
                }
            }
        }
        cells.add(cell.toString());
        return cells.toArray(new String[0]);
    }
}

package services.interfaces;

import models.PrintingHouse;

import java.io.IOException;
import java.nio.file.Path;

public interface IReportService {
    String generateReport(PrintingHouse house);
    void writeReport(String report, Path path) throws IOException;
    String readReport(Path file) throws IOException;
}

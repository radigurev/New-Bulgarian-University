package services.implementations;

import models.PrintingHouse;
import services.interfaces.IReportService;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

public class ReportService implements IReportService {
    @Override
    public String generateReport(PrintingHouse house) {
        StringBuilder sb = new StringBuilder();
        sb.append("=== Report for ").append(house.getName()).append(" ===\n");
        sb.append("Revenue: ").append(house.getLedger().getRevenue()).append("\n");
        sb.append("Paper costs: ").append(house.getLedger().getPaperCosts()).append("\n");
        sb.append("Salary costs: ").append(house.getLedger().getSalaryCosts()).append("\n");
        sb.append("Net profit: ").append(house.getLedger().getNetProfit()).append("\n");
        return sb.toString();
    }

    @Override
    public void writeReport(String report, Path path) throws IOException {
        Path parent = path.getParent();
        if (parent != null) Files.createDirectories(parent);

        Files.writeString(path, report);
    }

    @Override
    public String readReport(Path file) throws IOException {
        return Files.readString(file);
    }
}
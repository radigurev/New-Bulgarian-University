package tests;

import models.PrintingHouse;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import services.implementations.ReportService;

import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.*;

class ReportServiceTest {

    private ReportService service;
    private PrintingHouse house;

    @BeforeEach
    void setUp() {
        service = new ReportService();
        house = new PrintingHouse("house-1");
    }

    @Test
    void generateReport_notNullAndContainsHouseId() {
        String report = service.generateReport(house);

        assertNotNull(report);
        assertTrue(report.contains("house-1"));
    }

    @TempDir
    Path temp;

    @Test
    void writeAndReadReport_roundTrip() throws Exception {
        String content = "SOME REPORT";
        Path file = temp.resolve("report.txt");

        service.writeReport(content, file);
        String read = service.readReport(file);

        assertEquals(content, read);
    }

    @Test
    void readReport_fileNotFound_throws() {
        Path file = temp.resolve("missing.txt");

        assertThrows(Exception.class, () -> service.readReport(file));
    }
}

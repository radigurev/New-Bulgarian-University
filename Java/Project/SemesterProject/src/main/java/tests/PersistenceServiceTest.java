package tests;

import models.Manager;
import models.base.Employee;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import services.implementations.PersistenceService;

import java.math.BigDecimal;
import java.nio.file.Path;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

class PersistenceServiceTest {

    private PersistenceService service;

    @BeforeEach
    void setUp() {
        service = new PersistenceService();
    }

    @TempDir
    Path temp;

    @Test
    void saveAndLoadEmployees_roundTrip() throws Exception {
        List<Employee> staff = List.of(
                new Manager("e1", "Alice", BigDecimal.valueOf(1000), BigDecimal.valueOf(500)),
                new Manager("e2", "Bob", BigDecimal.valueOf(900), BigDecimal.valueOf(400))
        );

        Path file = temp.resolve("staff.bin");
        service.saveEmployees("./test/house-1", staff, file);

        List<Employee> loaded = service.loadEmployees("./test/house-1", file);

        assertEquals(staff.size(), loaded.size());
    }

    @Test
    void loadEmployees_fileNotFound_throwsException() {
        Path file = temp.resolve("missing.bin");

        assertThrows(Exception.class, () -> service.loadEmployees("house-1", file));
    }
}

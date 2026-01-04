package bg.nbu.transportcompany;

import bg.nbu.transportcompany.dto.*;
import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.service.*;
import bg.nbu.transportcompany.service.impl.*;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.math.BigDecimal;
import java.nio.file.Files;
import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.*;

public class FileExportImportTest extends BaseIntegrationTest {

    private final CompanyService companyService = new CompanyServiceImpl();
    private final ClientService clientService = new ClientServiceImpl();
    private final EmployeeService employeeService = new EmployeeServiceImpl();
    private final VehicleService vehicleService = new VehicleServiceImpl();
    private final TransportService transportService = new TransportServiceImpl();
    private final FileService fileService = new FileServiceImpl(transportService);

    @BeforeEach
    void setup() {
        DbTestUtil.truncateAll();
    }

    @Test
    void exportThenImport_works() throws Exception {
        TransportCompanyDTO company = companyService.create(TestData.company("TC"));
        ClientDTO client = clientService.create(TestData.client("Client1"));
        EmployeeDTO driver = employeeService.create(TestData.driver(company.getId(), "Ivan", DriverQualification.PASSENGER_TRANSPORT, new BigDecimal("2000")));
        VehicleDTO bus = vehicleService.create(TestData.bus(company.getId(), "CA0001AA", 50));

        transportService.create(TestData.passengerTransport(company.getId(), client.getId(), driver.getId(), bus.getId(), 40, new BigDecimal("100")));
        assertEquals(1, transportService.getAll().size());

        Path tmp = Files.createTempFile("transports", ".csv");
        fileService.exportTransportsToCsv(tmp);
        assertTrue(Files.size(tmp) > 0);

        DbTestUtil.truncateTransportsOnly();
        assertEquals(0, transportService.getAll().size());

        int imported = fileService.importTransportsFromCsv(tmp);
        assertEquals(1, imported);
        assertEquals(1, transportService.getAll().size());
    }
}

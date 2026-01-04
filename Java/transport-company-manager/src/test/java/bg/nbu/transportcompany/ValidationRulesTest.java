package bg.nbu.transportcompany;

import bg.nbu.transportcompany.dto.*;
import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.exception.InvalidDataException;
import bg.nbu.transportcompany.service.*;
import bg.nbu.transportcompany.service.impl.*;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.math.BigDecimal;

import static org.junit.jupiter.api.Assertions.*;

public class ValidationRulesTest extends BaseIntegrationTest {

    private final CompanyService companyService = new CompanyServiceImpl();
    private final ClientService clientService = new ClientServiceImpl();
    private final EmployeeService employeeService = new EmployeeServiceImpl();
    private final VehicleService vehicleService = new VehicleServiceImpl();
    private final TransportService transportService = new TransportServiceImpl();

    @BeforeEach
    void setup() {
        DbTestUtil.truncateAll();
    }

    @Test
    void passengerCountCannotExceedSeatCapacity() {
        TransportCompanyDTO company = companyService.create(TestData.company("TC"));
        ClientDTO client = clientService.create(TestData.client("Client1"));

        EmployeeDTO driver = employeeService.create(TestData.driver(company.getId(), "Ivan", DriverQualification.PASSENGER_TRANSPORT, new BigDecimal("2000")));
        VehicleDTO bus = vehicleService.create(TestData.bus(company.getId(), "CA0001AA", 10));

        TransportDTO t = TestData.passengerTransport(company.getId(), client.getId(), driver.getId(), bus.getId(), 11, new BigDecimal("50"));

        assertThrows(InvalidDataException.class, () -> transportService.create(t));
    }

    @Test
    void driverQualificationMustMatchVehicleType() {
        TransportCompanyDTO company = companyService.create(TestData.company("TC"));
        ClientDTO client = clientService.create(TestData.client("Client1"));

        EmployeeDTO driver = employeeService.create(TestData.driver(company.getId(), "Ivan", DriverQualification.PASSENGER_TRANSPORT, new BigDecimal("2000")));

        VehicleDTO truck = vehicleService.create(TestData.truck(company.getId(), "CA9999AA", 3000));

        TransportDTO t = TestData.cargoTransport(company.getId(), client.getId(), driver.getId(), truck.getId(), 1000, new BigDecimal("500"));
        assertThrows(InvalidDataException.class, () -> transportService.create(t));
    }
}

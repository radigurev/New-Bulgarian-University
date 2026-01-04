package bg.nbu.transportcompany;

import bg.nbu.transportcompany.dto.*;
import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;
import bg.nbu.transportcompany.report.DriverTransportCountDTO;
import bg.nbu.transportcompany.service.*;
import bg.nbu.transportcompany.service.impl.*;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

public class TransportFlowAndReportsTest extends BaseIntegrationTest {

    private final CompanyService companyService = new CompanyServiceImpl();
    private final ClientService clientService = new ClientServiceImpl();
    private final EmployeeService employeeService = new EmployeeServiceImpl();
    private final VehicleService vehicleService = new VehicleServiceImpl();
    private final TransportService transportService = new TransportServiceImpl();
    private final ReportService reportService = new ReportServiceImpl();

    @BeforeEach
    void setup() {
        DbTestUtil.truncateAll();
    }

    @Test
    void flow_createTransports_markPaid_reportsWork() {
        TransportCompanyDTO company = companyService.create(TestData.company("TC"));
        ClientDTO client = clientService.create(TestData.client("Client1"));

        EmployeeDTO driver = employeeService.create(TestData.driver(company.getId(), "Ivan", DriverQualification.PASSENGER_TRANSPORT, new BigDecimal("2000")));
        VehicleDTO bus = vehicleService.create(TestData.bus(company.getId(), "CA0001AA", 50));

        TransportDTO passenger = transportService.create(TestData.passengerTransport(company.getId(), client.getId(), driver.getId(), bus.getId(), 40, new BigDecimal("100")));
        assertNotNull(passenger.getId());
        assertEquals(1, reportService.getTotalTransportsCount());

        TransportDTO paid = transportService.markPaid(passenger.getId());
        assertEquals(bg.nbu.transportcompany.entity.PaymentStatus.PAID, paid.getPaymentStatus());

        assertEquals(new BigDecimal("100.00"), reportService.getTotalPaidRevenue());

        List<DriverTransportCountDTO> perDriver = reportService.getTransportsPerDriver();
        assertEquals(1, perDriver.size());
        assertEquals(driver.getId(), perDriver.get(0).getDriverId());
        assertEquals(1, perDriver.get(0).getTransportCount());

        List<CompanyRevenueDTO> perCompany = reportService.getRevenuePerCompany(LocalDate.now().minusDays(10), LocalDate.now());
        assertFalse(perCompany.isEmpty());
        assertEquals(company.getId(), perCompany.get(0).getCompanyId());
    }
}

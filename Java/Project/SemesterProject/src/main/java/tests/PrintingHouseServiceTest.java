package tests;

import exceptions.NoPaperException;
import exceptions.OverCapacityException;
import exceptions.UnsupportedPrintModeException;
import models.Manager;
import models.PrintingHouse;
import models.PrintingMachine;
import models.Publication;
import models.enums.PaperSize;
import models.enums.PaperType;
import models.enums.PrintType;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.junit.jupiter.MockitoExtension;
import repostories.PrintingHouseRepository;
import services.implementations.PrintingHouseService;
import services.interfaces.IPaperPricingPolicy;
import services.interfaces.IPersistenceService;
import services.interfaces.IReportService;

import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Path;
import java.util.List;
import java.util.NoSuchElementException;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class PrintingHouseServiceTest {

    private static final String HOUSE_ID = "house-1";

    @Mock IPaperPricingPolicy pricingPolicy;
    @Mock PrintingHouseRepository houseRepo;
    @Mock IReportService reportService;
    @Mock IPersistenceService persistenceService;

    private PrintingHouseService service;
    private PrintingHouse house;

    @BeforeEach
    void setUp() {
        house = new PrintingHouse(HOUSE_ID);
        when(houseRepo.get(HOUSE_ID)).thenReturn(house);
        service = new PrintingHouseService(pricingPolicy, houseRepo, reportService, persistenceService);
    }

    @Test
    void addMachine_colorMachine_supportsColorTrue() {
        service.addMachine(HOUSE_ID, "m1", PrintType.Color, 60, 200);

        PrintingMachine m = house.getMachines().stream()
                .filter(mm -> mm.getId().equals("m1")).findFirst().orElseThrow();
        assertTrue(m.isSupportsColor());
        assertEquals(200, m.getMaxCapacity());
    }

    @Test
    void addMachine_blackMachine_supportsColorFalse() {
        service.addMachine(HOUSE_ID, "m2", PrintType.Black, 30, 150);

        PrintingMachine m = house.getMachines().stream()
                .filter(mm -> mm.getId().equals("m2")).findFirst().orElseThrow();
        assertFalse(m.isSupportsColor());
    }

    @Test
    void hireEmployee_staffListUpdated() {
        Manager mgr = new Manager("e1", "Alice", BigDecimal.valueOf(1000), BigDecimal.valueOf(500));
        service.hireEmployee(HOUSE_ID, mgr);

        assertTrue(house.getStaff().contains(mgr));
    }

    @Test
    void buyPaper_costAddedToLedgerAndReturned() {
        when(pricingPolicy.priceFor(PaperType.GLOSSY, PaperSize.A4)).thenReturn(BigDecimal.valueOf(0.10));

        BigDecimal result = service.buyPaper(HOUSE_ID, PaperType.GLOSSY, PaperSize.A4, 100);

        assertEquals(BigDecimal.valueOf(10.0), result);
        assertEquals(BigDecimal.valueOf(10.0), house.getLedger().getPaperCosts());
    }

    @Test
    void loadMachine_withinCapacity_updatesLoadedSheets() throws OverCapacityException {
        PrintingMachine m = new PrintingMachine("m1", 100, false, 50);
        house.addMachine(m);

        service.loadMachine(HOUSE_ID, "m1", 80);

        assertEquals(80, m.getLoadedSheets());
    }

    @Test
    void loadMachine_exceedsCapacity_throwsException() {
        PrintingMachine m = new PrintingMachine("m1", 100, false, 50);
        house.addMachine(m);

        assertThrows(OverCapacityException.class,
                () -> service.loadMachine(HOUSE_ID, "m1", 101));
    }

    @Test
    void executePrintJob_blackMode_successful() throws Exception {
        PrintingMachine m = new PrintingMachine("m1", 200, false, 50);
        m.setLoadedSheets(200);
        house.addMachine(m);

        Publication pub = new Publication("Book", 50, 2, BigDecimal.valueOf(5), PrintType.Black);

        service.executePrintJob(HOUSE_ID, pub, "m1");

        assertEquals(100, m.getLoadedSheets());
        assertEquals(pub.totalRevenue(house.getDiscountRate(), house.getDiscountThreshold()), house.getLedger().getRevenue());
        assertEquals(1, m.getHistory().size());
    }

    @Test
    void executePrintJob_colorNotSupported_throwsException() {
        PrintingMachine m = new PrintingMachine("m1", 100, false, 50);
        m.setLoadedSheets(100);
        house.addMachine(m);

        Publication pub = new Publication("Magazine", 10, 5, BigDecimal.valueOf(3), PrintType.Color);

        assertThrows(UnsupportedPrintModeException.class,
                () -> service.executePrintJob(HOUSE_ID, pub, "m1"));
    }

    @Test
    void executePrintJob_notEnoughPaper_throwsException() {
        PrintingMachine m = new PrintingMachine("m1", 50, true, 50);
        m.setLoadedSheets(10);
        house.addMachine(m);

        Publication pub = new Publication("Flyer", 10, 2, BigDecimal.valueOf(1), PrintType.Color);

        assertThrows(NoPaperException.class,
                () -> service.executePrintJob(HOUSE_ID, pub, "m1"));
    }

    @Test
    void paySalaries_managerBonusApplied() {
        Manager mgr = new Manager("m1", "Bob", BigDecimal.valueOf(1000), BigDecimal.valueOf(500));
        house.addEmployee(mgr);

        house.getLedger().addRevenue(BigDecimal.valueOf(20_000));

        service.paySalaries(HOUSE_ID);

        assertEquals(BigDecimal.valueOf(1500), house.getLedger().getSalaryCosts());
    }

    @Test
    void paySalaries_managerBonusNotApplied() {
        Manager mgr = new Manager("m1", "Bob", BigDecimal.valueOf(1000), BigDecimal.valueOf(500));
        house.addEmployee(mgr);

        house.getLedger().addRevenue(BigDecimal.valueOf(5_000));

        service.paySalaries(HOUSE_ID);

        assertEquals(BigDecimal.valueOf(1000), house.getLedger().getSalaryCosts());
    }

    @Test
    void generateReport_invokesReportServiceAndReturnsContent() throws IOException {
        when(reportService.generateReport(house)).thenReturn("REPO");
        when(reportService.readReport(any())).thenReturn("REPO");

        String result = service.generateReport(HOUSE_ID);

        assertEquals("REPO", result);
        verify(reportService).writeReport(eq("REPO"), any());
    }

    @Test
    void saveEmployees_delegatesToPersistenceService() throws IOException {
        Manager mgr = new Manager("m1", "Bob", BigDecimal.valueOf(1000), BigDecimal.valueOf(500));
        house.addEmployee(mgr);

        Path path = Path.of("staff.bin");
        service.saveEmployees(HOUSE_ID, path);

        verify(persistenceService).saveEmployees(HOUSE_ID, house.getStaff(), path);
    }

    @Test
    void loadEmployees_replacesStaff() throws Exception {
        Manager oldMgr = new Manager("old", "Old", BigDecimal.valueOf(500), BigDecimal.ZERO);
        house.addEmployee(oldMgr);

        Manager newMgr = new Manager("new", "New", BigDecimal.valueOf(800), BigDecimal.ZERO);
        List<models.base.Employee> loaded = List.of(newMgr);

        Path path = Path.of("staff.bin");
        when(persistenceService.loadEmployees(HOUSE_ID, path)).thenReturn(loaded);

        service.loadEmployees(HOUSE_ID, path);

        assertEquals(loaded, house.getStaff());
    }

    @Test
    void getPrintedPages_returnsSumOfPages() {
        PrintingMachine m = new PrintingMachine("m1", 100, false, 50);
        m.recordPrint("Book1", 10, 2, false);
        m.recordPrint("Book2", 5, 4, false);
        house.addMachine(m);

        int pages = service.getPrintedPages(HOUSE_ID, "m1");

        assertEquals(10 * 2 + 5 * 4, pages);
    }

    @Test
    void getPrintedPages_machineNotFound_throwsException() {
        assertThrows(NoSuchElementException.class,
                () -> service.getPrintedPages(HOUSE_ID, "no-such"));
    }
}

package services.implementations;

import exceptions.NoPaperException;
import exceptions.OverCapacityException;
import exceptions.UnsupportedPrintModeException;
import models.PrintRecord;
import models.PrintingHouse;
import models.PrintingMachine;
import models.Publication;
import models.base.Employee;
import models.enums.PaperSize;
import models.enums.PaperType;
import models.enums.PrintType;
import repostories.PrintingHouseRepository;
import services.interfaces.IPaperPricingPolicy;
import services.interfaces.IPersistenceService;
import services.interfaces.IPrintingHouseService;
import services.interfaces.IReportService;

import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;
import java.util.NoSuchElementException;

public class PrintingHouseService implements IPrintingHouseService {
    private final IPaperPricingPolicy pricingPolicy;
    private final PrintingHouseRepository houseRepo;
    private final IReportService reportService;

    private final  IPersistenceService persistenceService;

    public PrintingHouseService(IPaperPricingPolicy pricingPolicy,
                                PrintingHouseRepository houseRepo,
                                IReportService reportService,
                                IPersistenceService persistenceService) {
        this.pricingPolicy = pricingPolicy;
        this.houseRepo = houseRepo;
        this.reportService = reportService;
        this.persistenceService = persistenceService;
    }

    @Override
    public void addMachine(String houseId,
                           String machineId,
                           PrintType printType,
                           int pagesPerMinute,
                           int capacity) {
        PrintingHouse house = houseRepo.get(houseId);
        boolean supportsColor = printType == PrintType.Color;
        house.addMachine(
                new PrintingMachine(machineId, capacity, supportsColor, pagesPerMinute)
        );
    }

    @Override
    public void hireEmployee(String houseId, Employee e) {
        houseRepo.get(houseId).addEmployee(e);
    }

    @Override
    public BigDecimal buyPaper(String houseId, PaperType type, PaperSize size, int sheets) {
        PrintingHouse house = houseRepo.get(houseId);
        BigDecimal costPerSheet = pricingPolicy.priceFor(type, size);
        BigDecimal totalCost = costPerSheet.multiply(BigDecimal.valueOf(sheets));
        house.getLedger().addPaperCost(totalCost);
        return totalCost;
    }

    @Override
    public void loadMachine(String houseId, String machineId, int sheets)
            throws OverCapacityException {
        PrintingMachine m = findMachine(houseId, machineId);
        int wouldBe = m.getLoadedSheets() + sheets;
        if (wouldBe > m.getMaxCapacity()) {
            throw new OverCapacityException(
                    "Cannot load " + sheets +
                            " sheets (would exceed capacity of " + m.getMaxCapacity() + ")" +
                            "Number of sheets already loaded: " + m.getLoadedSheets()
            );
        }
        m.setLoadedSheets(wouldBe);
    }

    @Override
    public void executePrintJob(String houseId,
                                Publication pub,
                                String machineId)
            throws UnsupportedPrintModeException, NoPaperException {
        PrintingHouse house = houseRepo.get(houseId);
        PrintingMachine m = findMachine(houseId, machineId);

        boolean color = pub.getMode() == PrintType.Color;
        if (color && !m.isSupportsColor()) {
            throw new UnsupportedPrintModeException(
                    "Machine " + machineId + " cannot print in color"
            );
        }

        int pagesNeeded = pub.getPagesPerCopy() * pub.getTotalCopies();
        if (m.getLoadedSheets() < pagesNeeded) {
            throw new NoPaperException("Need " + pagesNeeded +
                    " sheets but only " + m.getLoadedSheets() + " loaded");
        }

        m.setLoadedSheets(m.getLoadedSheets() - pagesNeeded);
        m.recordPrint(pub.getTitle(),
                pub.getTotalCopies(),
                pub.getPagesPerCopy(),
                color);

        BigDecimal revenue = pub.totalRevenue(
                house.getDiscountRate(),
                house.getDiscountThreshold()
        );
        house.getLedger().addRevenue(revenue);
    }

    @Override
    public void paySalaries(String houseId) {
        PrintingHouse house = houseRepo.get(houseId);
        house.getStaff().forEach(e -> {
            BigDecimal sal = e.getSalary(
                    house.getLedger().getRevenue(),
                    house.getManagerBonusThreshold()
            );
            house.getLedger().addSalaryCost(sal);
        });
    }

    @Override
    public String generateReport(String houseId) throws IOException {
        String report = reportService.generateReport(houseRepo.get(houseId));

        Path path = Paths.get("./"+houseId + "/report.txt");
        reportService.writeReport(report, path);
        return reportService.readReport(path);
    }

    @Override
    public void closePeriod(String houseId, Path reportPath) throws IOException {
        String rpt = generateReport(houseId);
        reportService.writeReport(rpt, reportPath);
    }

    @Override
    public String importReport(String houseId, Path reportFile) throws IOException {
        return reportService.readReport(reportFile);
    }

    @Override
    public int getPrintedPages(String houseId, String machineId) {
        PrintingHouse house = houseRepo.get(houseId);
        PrintingMachine m = house.getMachines().stream()
                .filter(machine -> machine.getId().equals(machineId))
                .findFirst()
                .orElseThrow(() -> new NoSuchElementException("Machine not found"));
        return m.getHistory().stream()
                .mapToInt(PrintRecord::totalPages)
                .sum();
    }

    @Override
    public void saveEmployees(String houseId, Path path) throws IOException {
        PrintingHouse house = houseRepo.get(houseId);
        persistenceService.saveEmployees(houseId,house.getStaff(), path);
    }

    @Override
    public void loadEmployees(String houseId, Path path) throws IOException, ClassNotFoundException {
        List<Employee> loaded = persistenceService.loadEmployees(houseId, path);
        PrintingHouse house = houseRepo.get(houseId);
        house.getStaff().clear();
        house.getStaff().addAll(loaded);
    }

    private PrintingMachine findMachine(String houseId, String machineId) {
        return houseRepo.get(houseId)
                .getMachines().stream()
                .filter(m -> m.getId().equals(machineId))
                .findFirst()
                .orElseThrow(() -> new NoSuchElementException(
                        "Machine '" + machineId + "' not found in house '" + houseId + "'"
                ));
    }
}
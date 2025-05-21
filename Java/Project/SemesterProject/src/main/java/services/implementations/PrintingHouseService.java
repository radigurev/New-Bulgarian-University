package services.implementations;

import models.PrintingHouse;
import models.PrintingMachine;
import models.Publication;
import models.base.Employee;
import models.enums.PaperSize;
import models.enums.PaperType;
import repostories.PrintingHouseRepository;
import services.interfaces.IPaperPricingPolicy;
import services.interfaces.IPersistenceService;
import services.interfaces.IPrintingHouseService;
import services.interfaces.IReportService;

import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Path;

public class PrintingHouseService implements IPrintingHouseService {
    private final IPaperPricingPolicy pricingPolicy;
    private final PrintingHouseRepository houseRepo;
    private final IPersistenceService persistence;
    private final IReportService reportService;

    public PrintingHouseService(IPaperPricingPolicy pricingPolicy,
                                    PrintingHouseRepository houseRepo,
                                    IPersistenceService persistence,
                                    IReportService reportService) {
        this.pricingPolicy = pricingPolicy;
        this.houseRepo = houseRepo;
        this.persistence = persistence;
        this.reportService = reportService;
    }

    @Override
    public void addMachine(String houseId, String machineId) {
        PrintingHouse house = houseRepo.get(houseId);
        house.addMachine(new PrintingMachine(machineId));
    }

    @Override
    public void hireEmployee(String houseId, Employee e) {
        PrintingHouse house = houseRepo.get(houseId);
        house.addEmployee(e);
    }

    @Override
    public BigDecimal buyPaper(String houseId, PaperType type, PaperSize size, int sheets) {
        PrintingHouse house = houseRepo.get(houseId);
        BigDecimal cost = pricingPolicy.priceFor(type, size).multiply(BigDecimal.valueOf(sheets));
        house.getLedger().addPaperCost(cost);
        return cost;
    }

    @Override
    public void loadMachine(String houseId, String machineId, int sheets) {
        PrintingHouse house = houseRepo.get(houseId);
        house.getMachines().stream()
                .filter(m -> m.getId().equals(machineId))
                .findFirst()
                .orElseThrow()
                .loadPaper(sheets);
    }

    @Override
    public void executePrintJob(String houseId, Publication pub, String machineId) throws Exception {
        PrintingHouse house = houseRepo.get(houseId);
        PrintingMachine machine = house.getMachines().stream()
                .filter(m -> m.getId().equals(machineId))
                .findFirst().orElseThrow();
        machine.print(pub, pub.getTotalCopies());
        BigDecimal revenue = pub.totalRevenue(house.getDiscountRate(), house.getDiscountThreshold());
        house.getLedger().addRevenue(revenue);
    }

    @Override
    public void paySalaries(String houseId) {
        PrintingHouse house = houseRepo.get(houseId);
        for (Employee e : house.getStaff()) {
            BigDecimal salary = e.getSalary(house.getLedger().getRevenue(), house.getManagerBonusThreshold());
            house.getLedger().addSalaryCost(salary);
        }
    }

    @Override
    public String generateReport(String houseId) {
        return reportService.generateReport(houseRepo.get(houseId));
    }

    @Override
    public void closePeriod(String houseId, Path reportPath) throws IOException {
        String report = generateReport(houseId);
        reportService.writeReport(report, reportPath);
    }

    @Override
    public String importReport(String houseId, Path reportFile) throws IOException {
        return reportService.readReport(reportFile);
    }
}
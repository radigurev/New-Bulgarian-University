package org.example;

import models.Manager;
import models.Operator;
import models.PrintingHouse;
import models.Publication;
import models.base.Employee;
import models.enums.PaperSize;
import models.enums.PaperType;
import repostories.PrintingHouseRepository;
import repostories.implementations.PrintingHouseRepositoryImpl;
import services.implementations.PersistenceService;
import services.implementations.PrintingHouseService;
import services.implementations.ReportService;
import services.interfaces.IPaperPricingPolicy;
import services.interfaces.IPersistenceService;
import services.interfaces.IPrintingHouseService;
import services.interfaces.IReportService;
import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;

public class Main {
    public static void main(String[] args) {
        try {
            // 1. Setup pricing policy
            IPaperPricingPolicy policy = (type, size) -> {
                // base prices for A5
                BigDecimal base;
                switch (type) {
                    case GLOSSY: base = BigDecimal.valueOf(0.10); break;
                    case MATTE:  base = BigDecimal.valueOf(0.08); break;
                    default:     base = BigDecimal.valueOf(0.05);
                }
                // size multiplier: A5=1, A4=1.2, A3=1.4, A2=1.6, A1=1.8
                BigDecimal multiplier;
                switch (size) {
                    case A4: multiplier = BigDecimal.valueOf(1.2); break;
                    case A3: multiplier = BigDecimal.valueOf(1.4); break;
                    case A2: multiplier = BigDecimal.valueOf(1.6); break;
                    case A1: multiplier = BigDecimal.valueOf(1.8); break;
                    default: multiplier = BigDecimal.ONE;
                }
                return base.multiply(multiplier);
            };

            // 2. Instantiate services and repository
            PrintingHouseRepository repo = new PrintingHouseRepositoryImpl();
            IPersistenceService persistence = new PersistenceService();
            IReportService reportService = new ReportService();
            IPrintingHouseService service = new PrintingHouseService(
                    policy, repo, persistence, reportService
            );

            // 3. Create a printing house
            PrintingHouse house = new PrintingHouse("My House");
            repo.save(house);

            // 4. Add machines
            service.addMachine("My House", "PM1");
            service.addMachine("My House", "PM2");

            // 5. Hire staff
            Operator op = new Operator("op1", "Alice", BigDecimal.valueOf(1000));
            Manager mgr = new Manager("mgr1", "Bob", BigDecimal.valueOf(1500), BigDecimal.valueOf(500));
            service.hireEmployee("My House", op);
            service.hireEmployee("My House", mgr);

            // 6. Purchase paper
            BigDecimal cost = service.buyPaper("My House", PaperType.GLOSSY, PaperSize.A4, 2000);
            System.out.println("Purchased paper cost: " + cost);

            // 7. Load machines
            service.loadMachine("My House", "PM1", 1000);
            service.loadMachine("My House", "PM2", 1000);

            // 8. Print publications
            Publication book = new Publication("Great Book", 800, BigDecimal.valueOf(2.5));
            service.executePrintJob("My House", book, "PM1");
            Publication poster = new Publication("Big Poster", 1200, BigDecimal.valueOf(1.0));
            service.executePrintJob("My House", poster, "PM2");

            service.paySalaries("My House");

            // 10. Generate and display report
            String report = service.generateReport("My House");
            System.out.println("--- Report ---\n" + report);

            // 11. Save report to file and read it back
            Path reportPath = Paths.get("report.txt");
            service.closePeriod("My House", reportPath);
            String loadedReport = service.importReport("My House", reportPath);
            System.out.println("--- Loaded Report ---\n" + loadedReport);

            // 12. Serialize and deserialize employees
            Path empFile = Paths.get("employees.dat");
            persistence.saveEmployees("My House", house.getStaff(), empFile);
            List<Employee> loadedStaff = persistence.loadEmployees("My House", empFile);
            System.out.println("--- Loaded Employees ---");
            for (Employee e : loadedStaff) {
                System.out.println(e.getId() + ": " + e.getName());
            }

        } catch (IOException | ClassNotFoundException cnf) {
            System.err.println("I/O error: " + cnf.getMessage());
        } catch (Exception ex) {
            System.err.println("Error: " + ex.getMessage());
        }
    }
}
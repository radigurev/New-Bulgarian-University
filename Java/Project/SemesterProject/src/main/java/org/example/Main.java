package org.example;

import models.Manager;
import models.Operator;
import models.PrintingHouse;
import models.Publication;
import models.enums.PaperSize;
import models.enums.PaperType;
import models.enums.PrintType;
import repostories.PrintingHouseRepository;
import repostories.implementations.PrintingHouseRepositoryImpl;
import services.implementations.PersistenceService;
import services.implementations.PrintingHouseService;
import services.implementations.ReportService;
import services.interfaces.*;

import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Paths;
import exceptions.NoPaperException;
import exceptions.OverCapacityException;
import exceptions.UnsupportedPrintModeException;

import java.util.*;

public class Main {
    private static final Scanner scanner = new Scanner(System.in);
    private static IPrintingHouseService service;
    private static PrintingHouseRepository repo;
    public static void main(String[] args) throws IOException {
        init();
        while (true) {
            showMenu();
            int choice = readInt("Select an option: ");
            switch (choice) {
                case 1 -> showReport();
                case 2 -> addMachine();
                case 3 -> hireEmployee();
                case 4 -> buyPaper();
                case 5 -> loadMachine();
                case 6 -> printPublication();
                case 7 -> paySalaries();
                case 8 -> saveEmployees();
                case 9 -> loadEmployees();
                case 10 -> showMachinePages();
                case 0 -> {
                    System.out.println("Exiting. Goodbye!");
                    return;
                }
                default -> System.out.println("Invalid choice. Try again.");
            }
        }
    }

    private static void init() {
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
        repo = new PrintingHouseRepositoryImpl();
        IReportService reportService = new ReportService();
        IPersistenceService persistenceService = new PersistenceService();
        service = new PrintingHouseService(policy, repo, reportService, persistenceService);


        PrintingHouse house1 = new PrintingHouse("HouseA");
        PrintingHouse house2 = new PrintingHouse("HouseB");
        PrintingHouse house3 = new PrintingHouse("HouseC");

        repo.save(house1);
        repo.save(house2);
        repo.save(house3);

        service.addMachine("HouseA", "M1", PrintType.Color, 20, 500);
        service.addMachine("HouseA", "M2", PrintType.Black, 15, 300);

        service.addMachine("HouseB", "M3", PrintType.Black, 25, 400);
        service.addMachine("HouseB", "M4", PrintType.Color, 30, 600);

        service.addMachine("HouseC", "M5", PrintType.Black, 10, 200);
        service.addMachine("HouseC", "M6", PrintType.Color, 22, 350);

        house1.addEmployee(new Operator("op1", "Alice", new BigDecimal("1000")));
        house1.addEmployee(new Manager("mgr1", "Bob", new BigDecimal("1500"), new BigDecimal("500")));

        house2.addEmployee(new Operator("op2", "Charlie", new BigDecimal("900")));
        house2.addEmployee(new Manager("mgr2", "Diana", new BigDecimal("1600"), new BigDecimal("400")));

        house3.addEmployee(new Operator("op3", "Eve", new BigDecimal("1100")));
        house3.addEmployee(new Manager("mgr3", "Frank", new BigDecimal("1400"), new BigDecimal("600")));

        service.buyPaper("HouseA", PaperType.GLOSSY, PaperSize.A4, 1000);
        service.buyPaper("HouseB", PaperType.MATTE, PaperSize.A3, 1500);
        service.buyPaper("HouseC", PaperType.NEWSPAPER, PaperSize.A2, 1200);

        try {
            service.loadMachine("HouseA", "M1", 300);
            service.loadMachine("HouseB", "M3", 200);
            service.loadMachine("HouseC", "M5", 150);
        } catch (OverCapacityException e) {
        }
    }

    private static void showMenu() {
        System.out.println("\n=== Printing House Menu ===");
        System.out.println("1) Show report");
        System.out.println("2) Add machine");
        System.out.println("3) Hire employee");
        System.out.println("4) Buy paper");
        System.out.println("5) Load machine");
        System.out.println("6) Print publication");
        System.out.println("7) Pay salaries");
        System.out.println("8) Save employees");
        System.out.println("9) Load employees");
        System.out.println("10) Show pages printed by machine");
        System.out.println("0) Exit");
    }

    private static int readInt(String prompt) {
        System.out.print(prompt);
        while (!scanner.hasNextInt()) {
            scanner.next();
            System.out.print("Enter a number: ");
        }
        return scanner.nextInt();
    }

    private static String readString(String prompt) {
        System.out.print(prompt);
        return scanner.next();
    }

    private static void showReport() throws IOException {
        String storeId = readString("Store ID: ");
        System.out.println(service.generateReport(storeId));
    }

    private static void addMachine() {
        String storeId = readString("Store ID: ");
        String id = readString("Machine ID: ");
        int ppm = readInt("Pages/minute: ");
        int cap = readInt("Max capacity: ");
        System.out.print("Supports color? (y/n): ");
        boolean color = scanner.next().equalsIgnoreCase("y");
        service.addMachine(storeId, id, color ? PrintType.Color : PrintType.Black, ppm, cap);
    }

    private static void hireEmployee() {
        String storeId = readString("Store ID: ");
        String id = readString("Employee ID: ");
        String name = readString("Name: ");
        BigDecimal base = new BigDecimal(readString("Base salary: "));
        System.out.print("Is manager? (y/n): ");
        if (scanner.next().equalsIgnoreCase("y")) {
            BigDecimal bonus = new BigDecimal(readString("Bonus: "));
            repo.get(storeId).addEmployee(new Manager(id, name, base, bonus));
        } else {
            repo.get(storeId).addEmployee(new Operator(id, name, base));
        }
    }

    private static void buyPaper() {
        String storeId = readString("Store ID: ");
        PaperType type = PaperType.valueOf(readString("Type (MATTE,GLOSSY,NEWSPAPER): "));
        PaperSize size = PaperSize.valueOf(readString("Size (A5,A4,A3,A2,A1): "));
        int sheets = readInt("Sheets: ");

        BigDecimal cost = service.buyPaper(storeId, type, size, sheets);
        System.out.println("Cost: " + cost);
    }

    private static void loadMachine() {
        String storeId = readString("Store ID: ");
        String mid = readString("Machine ID: ");
        int sheets = readInt("Sheets to load: ");
        try {
            service.loadMachine(storeId, mid, sheets);
        } catch (OverCapacityException e) {
            System.out.println("Error: " + e.getMessage());
        }
    }

    private static void printPublication() {
        String storeId = readString("Store ID: ");
        String title = readString("Title: ");
        int copies = readInt("Copies: ");
        int pages = readInt("Pages per copy: ");
        BigDecimal price = new BigDecimal(readString("Unit price: "));
        PrintType mode = PrintType.valueOf(readString("Mode (Color/Black): "));
        String mid = readString("Machine ID: ");
        Publication pub = new Publication(title, copies, pages, price, mode);
        try {
            service.executePrintJob(storeId, pub, mid);
        } catch (UnsupportedPrintModeException | NoPaperException e) {
            System.out.println("Error: " + e.getMessage());
        }
    }

    private static void paySalaries() {
        String storeId = readString("Store ID: ");
        service.paySalaries(storeId);
        System.out.println("Salaries processed.");
    }

    private static void saveEmployees() {
        try {
            String storeId = readString("Store ID: ");
            service.saveEmployees(storeId, Paths.get("./"+storeId + "/employees.dat"));
            System.out.println("Employees saved.");
        } catch (IOException e) {
            System.out.println("Error: " + e.getMessage());
        }
    }

    private static void loadEmployees() {
        try {
            String storeId = readString("Store ID: ");
            service.loadEmployees(storeId, Paths.get("./"+storeId + "/employees.dat"));
            System.out.println("Employees loaded.");
        } catch (Exception e) {
            System.out.println("Error: " + e.getMessage());
        }
    }

    private static void showMachinePages() {
        String storeId = readString("Store ID: ");
        String mid = readString("Machine ID: ");
        int pages = service.getPrintedPages(storeId, mid);
        System.out.println("Total pages by " + mid + ": " + pages);
    }
}
